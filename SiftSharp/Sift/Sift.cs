using System.Linq;
using System;

namespace SiftSharp.Sift
{
    public class Sift
    {
        private int intervalsInOctave;
        private int numberOfOctaves;

        public Sift(int intervalsInOctave, int numberOfOctaves)
        {
            this.intervalsInOctave = intervalsInOctave;
            this.numberOfOctaves = numberOfOctaves;
        }

        /// <summary>
        /// Determines whether a pixel is a scale-space extremum by comparing it to
        /// it's 3x3 pixel neighborhood in current scale, prev and next
        /// </summary>
        /// <param name="dogPyramid" type="int[][][,]">DoGPyramid</param>
        /// <param name="x" type="int">x coordinate of feature</param>
        /// <param name="y" type="int">y coordinate of feature</param>
        /// <param name="octave" type="int">The octave the feature was found in</param>
        /// <param name="scale" type="int">The picture of the octave the feature was found in (interval)</param>
        /// <returns>Bool true or false for a point</returns>
        public bool IsExtremum(int[][][,] dogPyramid, int x, int y, int octave, int scale)
        {
            bool isMinimum = false;
            bool isMaximum = false;
            int featurePixel = dogPyramid[octave][scale][x, y];

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
                            dogPyramid[octave][scale + imgIndex][x + xIndex, y + yIndex])
                        {
                            isMaximum = true;
                        }
                        //If the pixel of feature is less than neighbor
                        else if (featurePixel <
                                 dogPyramid[octave][scale + imgIndex][x + xIndex, y + yIndex])
                        {
                            isMinimum = true;
                        }
                        //If both are true, then there is both values greater and less than the pixel,
                        //therefore it is neither maximum or minimum
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