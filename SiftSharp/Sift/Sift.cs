using System.Linq;
using System;

namespace SiftSharp.Sift
{
    public class Sift
    {
        private int levelsInOctave;
        private int numberOfOctaves;

        public Sift(int levelsInOctave, int numberOfOctaves)
        {
            this.levelsInOctave = levelsInOctave;
            this.numberOfOctaves = numberOfOctaves;
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

            float[][,] imagesInOctaves = Enumerable
                .Range(0, levelsInOctave)
                .Select(_ => new float[gaussWidth, gaussHeight])
                .ToArray();

            float[][][,] result = Enumerable
                .Range(0, numberOfOctaves)
                .Select(_ => imagesInOctaves)
                .ToArray();

            Image[][] dogPyramid = new Image[numberOfOctaves][];

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
    }
}
