using System.Linq;
using System;

namespace SiftSharp.SIFT
{
    public class Sift
    {
        public Sift()
        {
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
                    if (i > 0 && i < width - 1 && i > 0 && i < height - 1)
                    {
                        double dx = inputConverted[x + i + 1, y + j] - inputConverted[x + i - 1, y + j];
                        double dy = inputConverted[x + i, y + j - 1] - inputConverted[x + i, y + j + 1];

                        // Calc magnitude and orientation from dx and dy
                        double magnitude = Math.Sqrt(dx * dx + dy * dy);
                        double orientation = Math.Atan2(dy, dx);

                        // Create a weight for bin ( e^( -(x^2 + y^2) / (2*sigma^2) ) )
                        double weight = Math.Exp(-(i * i + j * j) / exponential_denom);

                        // Determine which bin we need
                        int bin = (int)Math.Round(bins * (orientation + Math.PI) / (Math.PI * 2));

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