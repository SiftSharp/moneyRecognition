using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp {
    class BoyeImage {
        private Bitmap bitmapInput;
        private int[,] img;

        public  BoyeImage(string path) {
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
        public void convolve() { }
        public void crossCorrelate() { }
        public void sobel() { }
    }
}
