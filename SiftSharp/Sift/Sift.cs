using System.Linq;

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
        /// Creates DoG in Scale Space from Gaussian Pyramid in Scale Space
        /// </summary>
        /// <param name="gaussPyramid" type="int[][][,]">Gauss pyramid</param>
        /// <returns>int[][][,] DoG pyramid</returns>
        public int[][][,] BuildDogPyramid(int[][][,] gaussPyramid)
        {
            int gaussWidth = gaussPyramid[0][0].GetLength(0);
            int gaussHeight = gaussPyramid[0][0].GetLength(1);

            int[][,] imagesInOctaves = Enumerable
                .Range(0, intervalsInOctave - 1)
                .Select(_ => new int[gaussWidth, gaussHeight])
                .ToArray();

            int[][][,] result = Enumerable
                .Range(0, numberOfOctaves)
                .Select(_ => imagesInOctaves)
                .ToArray();

            //For each octave
            for (int octave = 0; octave < numberOfOctaves; octave++)
            {
                //For each picture in each octave
                for (int img = 1; img < intervalsInOctave; img++)
                {
                    //For each y in the image
                    for (int y = 0; y < gaussPyramid[octave][img].GetLength(1); y++)
                    {
                        //For each x in the image
                        for (int x = 0; x < gaussPyramid[octave][img].GetLength(0); x++)
                        {
                            //Subtract each pixel x,y in current image indexed from one with previous for each level
                            result[octave][img-1][x, y] =
                                gaussPyramid[octave][img][x, y] - gaussPyramid[octave][img - 1][x, y];
                        }
                    }
                }
            }
            return result;
        }
    }
}