using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp {
    class Image {
        const int GetWidth = 0;
        const int GetHeight = 1;
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


        public Bitmap Resize(Size newSize)
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
        public Bitmap Scale(double scalePercent)
        {
            Size size;
            size = new Size(bitmapInput.Width, bitmapInput.Height); //Create Size object.

            if (Math.Abs(scalePercent - 0.1) < double.Epsilon) // The smallest possible float number.
            {
                return Resize(size);
            }
            else
            {   // Scales Size object.
                double s = scalePercent / 100;
                size.Width = (int)(size.Width * s);
                size.Height = (int)(size.Height * s);
            }
            
            return Resize(size);
        }

        /* Removes every second pixel from image array. Render image half size.*/
        public float [,] Downsample()
        {
            int a = -2, b;
            int new_width = img.GetLength(GetWidth) / 2;               // Precompute new width and height
            int new_height = img.GetLength(GetHeight) / 2;             // from old array.
            float[,] outputArray = new float[new_width, new_height];
            for (int i = 0; i < new_width; i++)
            {
                a += 2;                               // Increase a.
                b = -2;                               // Reset b.
                for (int j = 0; j < new_height; j++)
                {
                    b += 2;                           // Every second pixel.
                    outputArray[i, j] = img[a, b];    // Copy old array to new.
                }
            }
            return outputArray;                       // Return output array.
        }
        public void gaussian() { }
        public void convolve() { }
        public void crossCorrelate() { }
        public void sobel() { }
    }
}
