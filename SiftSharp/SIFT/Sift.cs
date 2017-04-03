using System.Linq;
using System;

namespace SiftSharp.SIFT
{
    public class Sift
    {
        int levelsInOctave;
        int numberOfOctaves;
        Image input;

        public Sift(Image input, int levelsInOctave, int numberOfOctaves)
        {
            this.input = input;
            this.levelsInOctave = levelsInOctave;
            this.numberOfOctaves = numberOfOctaves;
        }

        /// <summary>
        /// Builds a Gaussian pyramid
        /// </summary>
        /// <param name="input">Image to build pyramid on</param>
        /// <param name="octaves">Octaves to build</param>
        /// <param name="levels">Levels per octave to filter</param>
        /// <returns>Gaussian pyramid</returns>
        public Image[][] GaussianPyramid(Image input, int octaves, int levels)
        {
            // Derived from Lowe 2004
            float initialSigma = (float)Math.Sqrt(2);
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
                float sigma = initialSigma * ((o + 1) * sigmaFactor);

                for (int l = 0; l < levels + extremaFactor; l++)
                {
                    if(o == 0 && l == 0)
                    {
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
        public Image[][] BuildDogPyramid(Image[][] gaussPyramid)
        {
            int gaussWidth = gaussPyramid[0][0].Get().GetLength(0);
            int gaussHeight = gaussPyramid[0][0].Get().GetLength(1);
            Image[][] dogPyramid = new Image[numberOfOctaves][];
            float[][][,] result = new float[numberOfOctaves][][,];

            //For each octave
            for (int octave = 0; octave < numberOfOctaves; octave++)
            {
                //For each picture in each octave (from 1 to +1 because of 2 extra layers (s+3 in gaussPyr))
                for (int level = 1; level < levelsInOctave + 1; level++)
                {
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
                            result[octave][level-1][x, y] =
                                currentImage[x, y] - prevImage[x, y];
                        }
                    }
                    dogPyramid[octave][level] = new Image(result[octave][level - 1]);
                }
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
        public double[] Histogram(Image input, int x, int y, int bins, int radius, double sigma)
        {
            // Histogram to be returned
            double[] histogram = new double[bins];
            // Get float array from image
            float[,] inputConverted = input.Get();

            // Save width/height
            int width = input.Get().GetLength(0);
            int height = input.Get().GetLength(1);

            // Exponential denomenator
            double exponential_denom = 2.0 * sigma * sigma;
            
            // from radius left of to radius right
            for (int i = -radius; i <= radius; i++)
            {
                // from radius above of to radius beneath
                for (int j = -radius; j <= radius; j++)
                {
                    // Make sure that pixel is within image
                    if (i > 0 && i < width - 1 && j > 0 && j < height - 1)
                    {
                        double dx = inputConverted[x + i + 1, y + j] - inputConverted[x + i - 1, y + j];
                        double dy = inputConverted[x + i, y + j - 1] - inputConverted[x + i, y + j + 1];

                        // Calc magnitude and orientation from dx and dy
                        double magnitude = Math.Sqrt(dx * dx + dy * dy);
                        double orientation = Math.Atan2(dy, dx);

                        // Create a weight for bin ( e^( -(x^2 + y^2) / (2*sigma^2) ) )
                        double weight = Math.Exp(-(i * i + j * j) / exponential_denom);

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
        /// Determines whether a pixel is a scale-space extremum by comparing it to
        /// it's 3x3 pixel neighborhood in current scale, prev and next
        /// </summary>
        /// <param name="dogPyramid" type="int[][][,]">DoGPyramid</param>
        /// <param name="x" type="int">x coordinate of feature</param>
        /// <param name="y" type="int">y coordinate of feature</param>
        /// <param name="octave" type="int">The octave the feature was found in</param>
        /// <param name="level" type="int">The picture of the octave the feature was found in</param>
        /// <returns>Bool true or false for a point</returns>
        public bool IsExtremum(Image[][] dogPyramid, int x, int y, int octave, int level)
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
                        //If the pixel of feature is greather than neighbor
                        if (featurePixel >
                            dogPyramid[octave][level + imgIndex].Get()[x + xIndex, y + yIndex])
                        {
                            isMaximum = true;
                        }
                        //If the pixel of feature is less than neighbor
                        else if (featurePixel <
                                 dogPyramid[octave][level + imgIndex].Get()[x + xIndex, y + yIndex])
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
