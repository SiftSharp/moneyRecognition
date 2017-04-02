using System;

namespace SiftSharp.Sift
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
        Image[][] GaussianPyramid(Image input, int octaves, int levels)
        {
            // Derived from Lowe 2004
            float initialSigma = (float) Math.Sqrt(2);
            // factor between sigma of each blurred image
            float sigmaFactor = (float) Math.Pow(2.0, 1.0 / levels);
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
                float sigma = initialSigma * (o * sigmaFactor);

                for (int l = 0; l < levels + extremaFactor; l++)
                {
                    // Blur image with sigma
                    pyramid[o][l] = tempImage.Clone().Gaussian(sigma);

                    // Step up sigma
                    sigma *= sigmaFactor;
                }

                // Downsample image by factore of 2
                tempImage = tempImage.Clone().Downsample();
            }

            return pyramid;
        }
    }
}