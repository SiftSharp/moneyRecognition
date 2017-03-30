using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp {
    class Image {
        private Bitmap bitmapInput;
        private int[,] img;

        public  Image(string path) {
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


        public Bitmap resize(Size newSize)
        {
            try
            { 
                Bitmap b = new Bitmap(newSize.Width, newSize.Height); // Create a new bitmap.
                using (Graphics g = Graphics.FromImage(b))  // Graphics object from bitmap.
                {
                    // sets interpolationmode to bicubic.
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    // interpolate the image.
                    g.DrawImage(bitmapInput , 0, 0, newSize.Width, newSize.Height);
                }
                return b; // returning the resized bitmap.
            }
            catch  // Errorhandeling here.
            {
                Console.WriteLine("Bitmap could not be resized"); // Error msg.
                return bitmapInput; // Return the non resized bitmap.
            }
        }

        /*
         Creates Size object, scales it and calls resize.
             */
        public Bitmap scale(double scalePercent)
        {
            Size size;
            size = new Size(bitmapInput.Width, bitmapInput.Height); //Create Size object.

            if (Math.Abs(scalePercent - 0.1) < double.Epsilon) // The smallest possible float number.
            {
                return resize(size);
            }
            else
            {   // Scales Size object.
                double s = scalePercent / 100;
                size.Width = (int)(size.Width * s);
                size.Height = (int)(size.Height * s);
            }
            
            return resize(size);
        }

        public int [,] downsample(int[,] img, int width, int height)
        {
            int a = 0, b = 0;
            int[,] aout = new int[width/2,height/2];
            for (int i = 0; i <= height; i++)
            {
                for (int j = 0; j <= width; j++)
                {
                    aout[i, j] = img[a, b];
                    b+=2;
                }
                a+=2;
            }
            return aout;
        }
        public void gaussian() { }
        public void convolve() { }
        public void crossCorrelate() { }
        public void sobel() { }
    }
}
