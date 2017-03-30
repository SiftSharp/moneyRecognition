using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

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

        public Image()
        {

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
<<<<<<< HEAD
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
                .Range(0, numberOfKernels)//From 0 to the number of kernels
                .Select(_ =>  new float[imageWidth, imageHeight])//Create new float
                .ToArray();//Make it an array

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
            //Loop through each pixel in array from left to right
            for (int y = 0; y < arrayHeight; y++)
            {
                for (int x = 0; x < arrayWidth; x++)
                {
                    //Convert generic type to string and thereafter parse the string to float
                    arrayAsFloat[x,y] = float.Parse(genericArray[x,y].ToString());
                }
            }
            return arrayAsFloat;
        }

        /// <summary>
        /// Calculates sum of filtered neighborhood and returns new convolved or cross correlated pixel
        /// </summary>
        /// <param name="image" type="I[,]">Image matrix</param>
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

            //Loop through kernel from negative index away from center to positive index away from center
            for (int kernelY = -kernelCenter; kernelY <= kernelCenter; kernelY++)
            {
                for (int kernelX =  -kernelCenter; kernelX <= kernelCenter; kernelX++)
                {
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
                    else throw new Exception("Please provide the type of window slide");
                }
            }
            return sum;
        }
=======

        /// <summary>
        /// This is a helper method for overloading params to Guassian funcion.
        /// This means that if you only provide a sigma value, then the
        /// Guassian call will be of a kernel size on 3 and the
        /// object data-stream.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        public void Gaussian(float sigma)
        {
             Gaussian(sigma, 3, this.img); // Calls the Gaussian method with 3 inputs.
        }

        /// <summary>
        /// This is a helper method for overloading params to Guassian funcion.
        /// This means that if you only provide a sigma value, and a kernel
        /// size then the Guassian call will be of the object data-stream.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <param name="size">The kernel size</param>
        public void Gaussian(float sigma, int size)
        {
            Gaussian(sigma, size, this.img); // Calls the Gaussian method with 3 inputs.
        }

        /// <summary>
        /// This is the main Gaussian method, where all params are required.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <param name="size">The kernel size</param>
        /// <param name="stream">The data-stream of the image</param>
        public void Gaussian(float sigma, int size, int [,] stream)
        {

            double[ , ] kernel = new double[size, size]; // Creates a new 2d array for the kernel (The matrix of the kernel)
            int kernelRadius = size / 2; // Sets the radius, this is needed since the middel of the kernel is cosidered [0,0]

            // The gaussian function is (1 / (2*PI*sigma^2))*e^(-1*(x^2+y^2)/(2*sigma^2))
            // For different purposes this equation is substracted into sub-parts so that,
            // The new equation is c*e^(-1*(x^2+y^2)/k), this means that:
            // (1 / (2*PI*sigma^2)) is substituded with c and
            // (2*sigma^2) is substituded with k.
            // That allows the calculation of c and k to only happend once per method call.

            double c = 1 / (2 * Math.PI * (sigma * sigma)); // Here c is calculated.
            double k = 2 * sigma * sigma; // Here k is calculated

            double accumulatedSum = 0.0; // This is the accumulated sum, which is used to normalize the data later on.

            for (int y = -kernelRadius; y <= kernelRadius; y++) // loop through the kernel from bottom row to top
                {
                    for (int x = -kernelRadius; x <= kernelRadius; x++) // loop through the columns from right to left
                    {
                        double value = c * Math.Exp(-1 * ((y * y + x * x) / (k))); // Calculate the gaussian value of the specific point.
                        accumulatedSum += value; // adds the value to the accumulated sum.
                        kernel[y + kernelRadius, x + kernelRadius] = value; // stores the value in the kernel-matrix.
                    }
                }

           // Here we loop through the kernel in order to normalize all the data.
           for (int y = 0; y < size; y++) // Loops through the rows top to bottom.
                {
                    for (int x = 0; x < size; x++) // Loops through the columns left to right.
                    {
                        kernel[y, x] = kernel[y, x] * (1.0 / accumulatedSum); //Normalizes the data by deviding it with the accumulated sum.
                    }
                }

           // Now we send the data-stream + the kernel to the convolution operation and return its value.
           //return convolve(stream, kernel);

           // This next part is just to console write the kernel,
           // This needs to be deleted before production.
            Console.WriteLine("kernel:");
            for (int y = 0; y < size; y++)
            {
                Console.Write("[ ");
                for (int x = 0; x < size; x++)
                {
                    Console.Write($"{kernel[y, x]}, ");
                }
                Console.Write("]");
                Console.WriteLine();
            }
        }

        public void convolve() { }
        public void crossCorrelate() { }
        public void sobel() { }
>>>>>>> 65ea957d6f37721cca5d3452ff23466644fbe2d8
    }
}
