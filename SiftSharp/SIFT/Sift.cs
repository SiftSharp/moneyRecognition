using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiftSharp.SIFT
{
    public class Sift
    {
        private static float initialSigma = 1.6F;
        private static float contrastTresh = 0.04F;
        private static float curvatureTresh = 10F;
        // Number of bins used for determining orientation of feaeture
        private static int orientationBins = 36;
        // determines gaussian sigma for orientation assignment
        public static float sigmaOrientationFactor = 1.5F;
        // determines the radius of the region used in orientation assignment
        public static float orientationRadius =  3.0F * sigmaOrientationFactor;
        
        private int levelsInOctave;
        private int numberOfOctaves;
        private Image input;

        public Feature[] features { get; private set; }
        public Image[][] gaussPyr { get; private set; }
        public Image[][] dogPyr { get; private set; }

        /// <summary>
        /// SIFT algorithm as described in Distinctive Image Features from
        /// Scale-Invariant Keypoints by Lowe 2004
        /// </summary>
        /// <param name="input" type="SiftSharp.Image">Image to find features in</param>
        /// <param name="levelsInOctave">Levels per octave</param>
        /// <param name="numberOfOctaves">Number of octaves</param>
        public Sift(Image input, int levelsInOctave, int numberOfOctaves)
        {
            this.input = input;
            this.levelsInOctave = levelsInOctave;
            this.numberOfOctaves = numberOfOctaves;

            // Build Gaussian Pyramid
            gaussPyr = GaussianPyramid(input, numberOfOctaves, levelsInOctave);

            // Build Difference-of-Gaussian Pyramid
            dogPyr = BuildDogPyramid(gaussPyr);

            // Find keypoints in Scale Space
            features = ScaleSpaceExtremas(dogPyr);

            // Assign scales to features
            features = FeatureScales(features, initialSigma, levelsInOctave);

            // Assign orientation to features
            features = FeatureOrientations(gaussPyr, features);
        }

        /// <summary>
        /// Find features in scale space based on various calcluations
        /// </summary>
        /// <param name="dogPyr">Difference-of-Gaussian pyramid</param>
        /// <returns>Array of features</returns>
        public Feature[] ScaleSpaceExtremas(Image[][] dogPyr)
        {
            // Octaves in the dog pyramid provided
            int octaves = dogPyr.GetLength(0);
            // Number of levels in the dog pyramid provided
            int levels = dogPyr[0].GetLength(0);

            List<Feature> listedKeypoints = 
                new List<Feature>();
            
            for (int o = 0; o < octaves; o++)
            {
                for (int l = 1; l < levels - 2; l++)
                {
                    float[,] currentLayer = dogPyr[o][l].Get();
                    for (int x = 1; x < currentLayer.GetLength(0) - 2; x++)
                    {
                        for (int y = 1; y < currentLayer.GetLength(1) - 2; y++)
                        {
                            if (dogPyr[o][l].Get()[x, y] > contrastTresh && IsExtremum(dogPyr, x, y, o, l))
                            {
                                Feature feat =
                                    Interpolation.InterpolatesExtremum(dogPyr, o, l, x, y, levels, curvatureTresh);

                                if(feat != null && IsTooEdgeLike(dogPyr[o][l],x,y,curvatureTresh))
                                {
                                    listedKeypoints.Add(feat);
                                }
                            }
                        }
                    }
               }
            }
            return listedKeypoints.ToArray();
        }

        /// <summary>
        /// Finds and assigns orientation to features
        /// </summary>
        /// <param name="gaussPyr">Difference-of-Gaussian pyramid</param>
        /// <param name="features">Array of features</param>
        /// <returns>Array of features with orientations</returns>
        public Feature[] FeatureOrientations(Image[][] gaussPyr, Feature[] features)
        {
            for (int i = 0; i < features.Length; i++)
            {
                Feature curFeat = features[i];
                double[] histogram = Histogram(
                    gaussPyr[curFeat.octave][curFeat.level],
                    (int)curFeat.x,
                    (int)curFeat.y,
                    orientationBins,
                    (int)Math.Round(orientationRadius * curFeat.scale),
                    sigmaOrientationFactor * curFeat.scale
                );
                
                int orientationIndex = histogram.ToList().IndexOf(histogram.Max());
                features[i].orientation = ((float)orientationIndex / orientationBins) * (2 * Math.PI);
            }
            return features;
        }

        /// <summary>
        /// Determines whether a feature is too edge like to be stable by
        /// computing the ratio of curvatures at that feature.
        /// Based on Section 4.1 in Lowe's paper.
        /// </summary>
        /// <param name="dogImage" type="Image">dogImage</param>
        /// <param name="x" type="int">x coordinate</param>
        /// <param name="y" type="int">y coordinate</param>
        /// <param name="curvatureThreshold" type="float">Curvature threshold</param>
        /// <returns>bool</returns>
        public bool IsTooEdgeLike(Image dogImage, int x, int y, float curvatureThreshold)
        {
            float trace = 0F, determinant = 0F;
            float[,] currentImage = dogImage.Get();
            float pixelVal = currentImage[x, y];

            //Partial derivatives
            float dxx = currentImage[x, y + 1] + currentImage[x, y - 1] - 2 * pixelVal;
            float dyy = currentImage[x + 1, y] + currentImage[x - 1, y] - 2 * pixelVal;
            float dxy = (currentImage[x + 1, y + 1]) -
                        (currentImage[x - 1, y + 1]) -
                        (currentImage[x - 1, y - 1]) / 4.0F;

            //Calculate trace & determinant
            trace = dxx + dyy;
            determinant = dxx * dyy - dxy * dxy;

            //Negative determinant = reject feature
            //If true => contrast great enough
            if (determinant > 0 && ((trace * trace) / determinant) < 
                ((curvatureThreshold + 1.0) * (curvatureThreshold + 1.0) /
                curvatureThreshold))
            {
                return false;
            }

            //If not return too edge like
            return true;
        }

        /// <summary>
        /// Finds and assigns scale to array of features
        /// </summary>
        /// <param name="features">Array of Features</param>
        /// <param name="sigma">Sigma</param>
        /// <param name="numberOflevels">Number of levels per octave</param>
        /// <returns></returns>
        public static Feature[] FeatureScales(Feature[] features, float sigma, int numberOflevels)
        {
            for (int i = 0; i < features.Length; i++)
            {
                features[i].scale = sigma * Math.Pow(2.0, 
                    (features[i].octave) + ((features[i].level + features[i].subLevel) / numberOflevels));
            }
            return features;
        }
        
        /// <summary>
        /// Builds a Gaussian pyramid
        /// </summary>
        /// <param name="input">Image to build pyramid on</param>
        /// <param name="octaves">Octaves to build</param>
        /// <param name="levels">Levels per octave to filter</param>
        /// <returns>Gaussian pyramid</returns>
        public static Image[][] GaussianPyramid(Image input, int octaves, int levels)
        {
            double[] sigmas = new double[levels + 3];

            // https://github.com/robwhess/opensift/blob/master/src/sift.c#L252
            // precompute Gaussian sigmas using the following formula:
            // \sigma_{total}^2 = \sigma_{i}^2 + \sigma_{i-1}^2
            // sig[i] is the incremental sigma value needed to compute 
            // the actual sigma of level i. Keeping track of incremental
            // sigmas vs. total sigmas keeps the gaussian kernel small.
            float k = (float)Math.Pow(2.0, 1.0 / levels);

            sigmas[0] = initialSigma;
            sigmas[1] = initialSigma * Math.Sqrt(k * k - 1);

            for (int i = 2; i < levels + 3; i++)
            {
                sigmas[i] = sigmas[i - 1] * k;
            }

            // factor between sigma of each blurred image
            float sigmaFactor = (float)Math.Pow(2.0, 1.0 / levels);
            // Clone image into temporary image
            Image tempImage = input.Clone();

            Image[][] pyramid = new Image[octaves][];

            // https://stackoverflow.com/questions/2704844/how-to-use-dog-pyramid-in-sift
            // https://github.com/aminert/CBIR/blob/master/SIFT.m#L120
            // In order to detect keypoints on s intervals per octave, 
            // we must generate s+3 blurred images in the gaussian 
            // pyramid. This is becuase s + 3 blurred images generates
            // s + 2 DOG images, and two images are needed (one at the 
            // highest and one lowest scales of the octave) for 
            // extrema detection.
            int extremaFactor = 3;

            // Foreach octave and foreach level
            for (int o = 0; o < octaves; o++)
            {
                pyramid[o] = new Image[levels + extremaFactor];
                
                for (int l = 0; l < levels + extremaFactor; l++)
                {
                    if(o == 0 && l == 0)
                    {
                        // If first level on first octave
                        pyramid[o][l] = tempImage.Clone();
                    }
                    else if(l == 0)
                    {
                        // Downsample image by factore of 2
                        pyramid[o][l] = pyramid[o - 1][levels - 1].Clone().Downsample();
                    }else
                    {
                        // Blur image with sigma
                        pyramid[o][l] = pyramid[o][l - 1].Clone().Gaussian((float)sigmas[l]);
                    }
                }
            }

            return pyramid;
        }

        /// <summary>
        /// Creates DoG in Scale Space from Gaussian Pyramid in Scale Space
        /// </summary>
        /// <param name="gaussPyramid" type="int[][][,]">Gauss pyramid</param>
        /// <returns>int[][][,] DoG pyramid</returns>
        public static Image[][] BuildDogPyramid(Image[][] gaussPyramid)
        {
            // Number of levels in the gaussian pyramid provided
            int levels = gaussPyramid[0].GetLength(0);
            int octaves = gaussPyramid.GetLength(0);

            // DoG Pyramid
            Image[][] dogPyramid = new Image[octaves][];

            // Float array to be casted to Image
            float[][][,] result = new float[octaves][][,];

            //For each octave
            for (int octave = 0; octave < octaves; octave++)
            {
                // Get dimensions of current octave image size
                int gaussWidth = gaussPyramid[octave][0].Get().GetLength(0);
                int gaussHeight = gaussPyramid[octave][0].Get().GetLength(1);

                // Set size of octave
                result[octave] = new float[levels - 1][,];

                // Set size of octave in pyramid
                dogPyramid[octave] = new Image[levels - 1];

                //For each picture in each octave (from 1 to +1 because of 2 extra layers (s+3 in gaussPyr))

                //for (int level = 1; level < levels; level++)
                Parallel.For(1, levels, level =>
                {
                    result[octave][level - 1] = new float[gaussWidth, gaussHeight];

                    //Get image
                    float[,] currentImage = gaussPyramid[octave][level].Get();
                    float[,] prevImage = gaussPyramid[octave][level - 1].Get();

                    //For each y in the image
                    for (int y = 0; y < currentImage.GetLength(1); y++)
                    {
                        //For each x in the image
                        for (int x = 0; x < currentImage.GetLength(0); x++)
                        {
                            //Subtract each pixel x,y in current image indexed from one with previous for each level
                            result[octave][level - 1][x, y] =
                                Math.Abs(currentImage[x, y] - prevImage[x, y]);
                        }
                    }
                    dogPyramid[octave][level - 1] = new Image(result[octave][level - 1]);
                });
                //}
            }
            return dogPyramid;
        }

        /// <summary>
        /// Creates a gradient orientation histogram for a specific pixel
        /// </summary>
        /// <param name="input">Image to compute on</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="bins">Number of bins in histogram</param>
        /// <param name="radius">Radius of region over which the histogram is computed</param>
        /// <param name="sigma"></param>
        /// <returns>
        /// Returns a histogram array of defined bins representing orientations between 0 and 2 PI
        /// </returns>
        public static double[] Histogram(Image input, int x, int y, int bins, int radius, double sigma)
        {
            // Histogram to be returned
            double[] histogram = new double[bins];
            // Get float array from image
            float[,] inputConverted = input.Get();

            // Save width/height
            int width = input.Get().GetLength(0);
            int height = input.Get().GetLength(1);

            // Exponential denomenator
            double exponentialDenom = 2.0 * sigma * sigma;
            
            // from radius left of to radius right
            for (int i = -radius; i <= radius; i++)
            {
                // from radius above of to radius beneath
                for (int j = -radius; j <= radius; j++)
                {
                    // Make sure that pixel is within image
                    if (x + i > radius && x + i < width - radius && 
                        y + j > radius && y + j < height - radius)
                    {
                        double dx = inputConverted[x + i + 1, y + j] - inputConverted[x + i - 1, y + j];
                        double dy = inputConverted[x + i, y + j - 1] - inputConverted[x + i, y + j + 1];

                        // Calc magnitude and orientation from dx and dy
                        double magnitude = Math.Sqrt(dx * dx + dy * dy);
                        double orientation = Math.Atan2(dy, dx);

                        // Create a weight for bin ( e^( -(x^2 + y^2) / (2*sigma^2) ) )
                        double weight = Math.Exp(-(i * i + j * j) / exponentialDenom);

                        // We find the percentage of which the orientation fills a full
                        // circle. We do this by first ensuring that we don't end with 
                        // negative values by adding PI to our orientation. Afterwards
                        // we devide by by 2PI to get the percentage of the full circle. 
                        double percantageOfCircle = (orientation + Math.PI) / (Math.PI * 2);

                        // Determine which bin inwhich our magnitude should be added to
                        // by multiplying our pecentage by our number of bins and round
                        // to nearest integer, to find the respective bin.
                        int bin = (int)Math.Round(bins * percantageOfCircle);

                        // Set bin to zero if higher
                        bin = (bin < bins) ? bin : 0;

                        // Add to histogram
                        histogram[bin] += weight * magnitude;
                    }
                }
            }

            return histogram;
        }

        /// <summary>
        /// Finds most dominant orientation from built histogram
        /// based on input points.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bins"></param>
        /// <param name="radius"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        public int DominantOrientation(Image input, int x, int y, int bins, int radius, double sigma)
        {
            double[] histogram = Histogram(input, x, y, bins, radius, sigma);
            return histogram.ToList().IndexOf(histogram.Max());
        }

        /// <summary>
        /// Determines whether a pixel is a scale-space extremum by comparing it to
        /// it's 3x3 pixel neighborhood in current scale, prev and next
        /// </summary>
        /// <param name="dogPyramid" type="int[][][,]">DoGPyramid</param>
        /// <param name="x" type="int">x coordinate of feature</param>
        /// <param name="y" type="int">y coordinate of feature</param>
        /// <param name="octave" type="int">The octave the feature was found in</param>
        /// <param name="level" type="int">The picture of the octave the feature was found in</param>
        /// <returns>Bool true or false for a point</returns>
        public static bool IsExtremum(Image[][] dogPyramid, int x, int y, int octave, int level)
        {
            bool isMinimum = false;
            bool isMaximum = false;
            float[,] currentImage = dogPyramid[octave][level].Get();
            float featurePixel = currentImage[x, y];

            //For adjacent to next image
            for (int imgIndex = -1; imgIndex <= 1; imgIndex++)
            {
                //For each index away (3x3 neighborhood)
                for (int xIndex = -1; xIndex <= 1; xIndex++)
                {
                    for (int yIndex = -1; yIndex <= 1; yIndex++)
                    {
                        if (imgIndex == 0 && xIndex == 0 && yIndex == 0)
                        {
                            continue;
                        }

                        float levelImage = (dogPyramid[octave][level + imgIndex].Get()[x + xIndex, y + yIndex]);
                        
                        
                        //If the pixel of feature is greather than neighbor
                        if (featurePixel > levelImage)
                        {
                            isMaximum = true;
                        }
                        //If the pixel of feature is less than neighbor
                        else if (featurePixel < levelImage)
                        {
                            isMinimum = true;
                        }
                        //If both are true, then there is both values greater and less than the pixel,
                        // therefore it is neither maximum or minimum
                        if (isMaximum && isMinimum)
                        {
                            return false;
                        }
                    }
                }
            }
            return isMinimum || isMaximum;
        }
   }
}
