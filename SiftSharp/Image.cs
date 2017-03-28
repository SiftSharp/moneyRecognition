using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp {
     class Image {
        private Bitmap bitmapInput;
        private int[,] img;

        public Image(string path) {
            try {
                bitmapInput = new Bitmap(path);
            }
            catch (ArgumentException ex) {
                Console.WriteLine(
                    "{0}: {1}, probable cause is that the file wasn't found",
                    ex.GetType().Name,
                    ex.Message
                );
                return;
            }
            img = readImage(this.bitmapInput);
        }

        /// <summary>
        ///     Reads an input bitmap and generates
        ///     a B&W data output
        /// </summary>
        /// <param name="input">a bitmap image</param>
        /// <returns>Black and white two dimensional array of image</returns>
        public int[,] readImage(Bitmap input) {
            int width = input.Width,
                height = input.Height;
            int[,] output = new int[width, height];
            Color colors;
            LockBitmap inputLocked = new LockBitmap(input);
            inputLocked.LockBits();

            // Store grayscale value for each pixel
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    colors = inputLocked.GetPixel(x, y);
                    output[x, y] = (int)(
                        (colors.R + colors.G + colors.B) / 3.0F
                    );
                }
            }

            inputLocked.UnlockBits();

            return output;
        }
        public void resize() { }
        public void scale() { }
        public void gaussian() { }


        //SlideTypes: Flag used for SlidingWindow()
        //Used to specifiy whether Convolution or Cross Correlation should be applied when using SlidingWindow()
        [Flags]
        public enum SlideTypes
        {
            None,
            Convolution,
            CrossCorrelation
        }

        /// <summary>
        /// Performs Convolution or Cross correlation on image with user specified kernels
        /// </summary>
        /// <param name="image" type="double[,]">Image matrix</param>
        /// <param name="kernels" type="T[][,]">Array of kernels</param>
        /// <param name="slideType" type="enum">Convolution or Crosscorrelation flag</param>
        /// <returns>New image after filter applied</returns>
        public float[][,] SlidingWindow<I,T>(I[,] image, T[][,] kernels, SlideTypes slideType)
        {
            int imageHeight = image.GetLength(0);
            int imageWidth = image.GetLength(1);

            //Assumes that kernel size always will be same when multiple kernels should be applied
            int kernelHeight = kernels[0].GetLength(0), kernelWidth = kernels[0].GetLength(1);

            int kernelCenter = kernelHeight / 2;
            int numberOfKernels = kernels.GetLength(0);

            //Initialize array of 2D arrays with correct size
            float[][,] result = Enumerable
                .Range(0, numberOfKernels)
                .Select(_ =>  new float[imageWidth, imageHeight])
                .ToArray();

            //Loops through image pixels
            for (int y = kernelCenter; y < (imageHeight - kernelCenter); y++)
            {
                for (int x = kernelCenter; x < (imageWidth - kernelCenter); x++)
                {
                    //Loops through all kernels in kernels[] and calls ApplyKernel() with said kernel
                    for(int k = 0; k < numberOfKernels; k++)
                    {
                        result[k][x, y] = ApplyKernel(image, kernels[k], x, y, slideType);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Helper: Convert multidimensional generic to float[,]
        /// </summary>
        /// <param name="genericArray" type="T[,]">Generic matrix</param>
        /// <returns>returns float[,]</returns>
        public float[,] AsFloat<T>(T[,] genericArray)
        {
            int arrayHeight = genericArray.GetLength(0), arrayWidth = genericArray.GetLength(1);
            float[,] arrayAsFloat = new float[arrayWidth, arrayHeight];
            for (int y = 0; y < arrayHeight; y++)
            {
                for (int x = 0; x < arrayWidth; x++)
                {
                    arrayAsFloat[x,y] = float.Parse(genericArray[x,y].ToString());
                }
            }
            return arrayAsFloat;
        }

        /// <summary>
        /// Calculates sum of filtered neighborhood and returns new convolved or cross correlated pixel
        /// </summary>
        /// <param name="image" type="double[,]">Image matrix</param>
        /// <param name="kernel" type="T[,]">Kernel</param>
        /// <param name="x" type="int">pixel x coordinate from image</param>
        /// <param name="y" type="int">pixel y coordinate from image</param>
        /// <param name="slideType" type="enum">Convolution or Crosscorrelation flag</param>
        /// <returns>A pixel after summation of local neighborhood</returns>
        public float ApplyKernel<I,T>(I[,] image, T[,] kernel, int x, int y, SlideTypes slideType)
        {
            int imageHeight = image.GetLength(0), imageWidth = image.GetLength(1);
            int kernelHeight = kernel.GetLength(0), kernelWidth = kernel.GetLength(1);
            int kernelCenter = kernelHeight / 2;
            float sum = 0.0F;
            SlideTypes flags = SlideTypes.Convolution | SlideTypes.CrossCorrelation;

            //Loop through kernel
            for (int kernelY = -kernelCenter; kernelY <= kernelCenter; kernelY++) {
                for (int kernelX =  -kernelCenter; kernelX <= kernelCenter; kernelX++) {
                    if ((flags & SlideTypes.Convolution) == SlideTypes.Convolution)
                    {
                        //Convolution
                        sum += (AsFloat(image)[x + kernelX, y + kernelY] *
                                AsFloat(kernel)[kernelCenter - kernelX, kernelCenter - kernelY]);
                    }
                    else if ((flags & SlideTypes.CrossCorrelation) == SlideTypes.CrossCorrelation)
                    {
                        //Cross Correlation
                        sum += (AsFloat(image)[x - kernelX, y - kernelY] *
                                AsFloat(kernel)[kernelCenter + kernelX, kernelCenter + kernelY]);
                    }
                    else throw new Exception("Please provide the type of window slide.");
                }
            }
            return sum;
        }
    }
}
