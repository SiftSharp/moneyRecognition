using System;
using System.Collections.Generic;
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
    }
}
