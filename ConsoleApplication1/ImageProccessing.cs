using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApplication1 {
    internal class ImageProccessing {
        private Bitmap img;
        private LockBitmap bmp;
        public enum sobel{
            Horizontal,
            Vertical,
            DiagonalF,
            DiagonalB
        };
        
        public ImageProccessing(Bitmap img) {
            this.img = img;
            bmp = new LockBitmap(img);
        }

        public Bitmap build() {
            return img;
        }

        public ImageProccessing Sobel() {
            bmp.LockBits();

            Bitmap bmp1 = new Bitmap(bmp.Width, bmp.Height);

            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    
                    bmp1.SetPixel(x, y,
                        applyConvolutionKernel(
                            generateSobelKernel(sobel.Horizontal),
                            generateSobelKernel(sobel.Vertical),
                            x,
                            y
                       )
                   );
                }
            }

            Marshal.Copy(bmp1., bmp.Pixels, 0, bmp.Pixels.Length);
            bmp.UnlockBits();
            return this;
        }

        public ImageProccessing Gaussian(double weight, int kernelSize) {
            bmp.LockBits();

            // This is our Gaussian Convulated Kernel
            double[,] kernel = generateGaussianKernel(weight, kernelSize);
            //Bitmap oldBmp = bmp;

            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    bmp.SetPixel(x, y, applyConvolutionOnPixel(kernel, x, y));
                }
            }


            bmp.UnlockBits();
            return this;
        }

        private Color applyConvolutionKernel(double[,] kernelX, double[,] kernelY, int x, int y) {
            int kernelRadius = kernelX.GetLength(0) / 2;
            double[] sumX = { 0.0, 0.0, 0.0 };
            double[] sumY = { 0.0, 0.0, 0.0 };
            Color color;
            int amountOfActivePixels = 0;

            for (int Y = (y - kernelRadius); Y <= (y + kernelRadius); Y++) {
                for (int X = (x - kernelRadius); X <= (x + kernelRadius); X++) {

                    // Just drop values outside the image (you could do something else)
                    if (Y < 0 || X < 0 || Y + 1 > bmp.Height || X + 1 > bmp.Width) {
                        continue;
                    }

                    color = bmp.GetPixel(X, Y);

                    sumX[0] += color.R * kernelX[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sumX[1] += color.G * kernelX[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sumX[2] += color.B * kernelX[X - (x - kernelRadius), Y - (y - kernelRadius)];

                    sumY[0] += color.R * kernelY[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sumY[1] += color.G * kernelY[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sumY[2] += color.B * kernelY[X - (x - kernelRadius), Y - (y - kernelRadius)];

                    amountOfActivePixels++;
                }
            }
            
            for (int i = 0; i < sumX.GetLength(0); i++) {
                sumX[i] = Math.Sqrt((sumX[i] * sumX[i]) + (sumY[i] * sumY[i]));
                sumX[i] = sumX[i] > 255 ? 255 : sumX[i];
                sumX[i] = sumX[i] <   0 ?   0 : sumX[i];
            }

            return Color.FromArgb((int)sumX[0], (int)sumX[1], (int)sumX[2]);
        }

        private Color applyConvolutionOnPixel(double[,] kernel, int x, int y) {
            int kernelRadius = kernel.GetLength(0) / 2;
            double[] sum = { 0.0, 0.0, 0.0 };
            Color color;
            int amountOfActivePixels = 0;

            for (int Y = (y - kernelRadius); Y <= (y + kernelRadius); Y++) {
                for (int X = (x - kernelRadius); X <= (x + kernelRadius); X++) {

                    // Just drop values outside the image (you could do something else)
                    if (Y < 0 || X < 0 || Y + 1 > bmp.Height || X + 1 > bmp.Width) {
                        continue;
                    }

                    color = bmp.GetPixel(X, Y);

                    sum[0] += color.R * kernel[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sum[1] += color.G * kernel[X - (x - kernelRadius), Y - (y - kernelRadius)];
                    sum[2] += color.B * kernel[X - (x - kernelRadius), Y - (y - kernelRadius)];

                    amountOfActivePixels++;
                }
            }

            for(int i = 0; i < sum.GetLength(0); i++) {
                sum[i] = sum[i] > 255 ? 255 : sum[i];
                sum[i] = sum[i] < 0 ? 0 : sum[i];
            }

            return Color.FromArgb((int)sum[0], (int)sum[1], (int)sum[2]);
        }

        private static double[,] generateGaussianKernel(double weight, int size) {
            double[,] kernel = new double[size, size];
            int kernelRadius = size / 2;
            double sum = 0;

            for (int Y = -kernelRadius; Y <= kernelRadius; Y++) {
                for (int X = -kernelRadius; X <= kernelRadius; X++) {
                    kernel[X + kernelRadius, Y + kernelRadius] =
                        calculateKernelEntity(X, Y, weight);

                    sum += kernel[X + kernelRadius, Y + kernelRadius];
                }
            }

            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    kernel[x, y] *= 1.0 / sum;
                }
            }

            return kernel;
        }

        private double[,] generateSobelKernel(sobel direction) {
            switch (direction) {
                case sobel.Vertical:
                    return new double[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                case sobel.Horizontal:
                    return new double[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
                case sobel.DiagonalF:
                    return new double[,] { { -2, -1, 0 }, { -1, 0, 1 }, { 0, 1, 2 } };
                case sobel.DiagonalB:
                    return new double[,] { { 0, -1, -2 }, { 1, 0, -1 }, { 2, 1, 0 } };
                default:
                    return null;
            }
        }


        /**
         * Calculates the entity value for x, y
         * @param  x The X value for the matrix
         * @param  y The Y value for the matrix
         * @return   Calculated kernel entity for x,y
         */
        public static double calculateKernelEntity(int x, int y, double weight) {
            // (1 / (2 * pi * (weight) ^ 2)) * e ^ (-((x ^ 2 + y ^ 2) / (2 * (weight) ^ 2)))
            double a = 1 / (2 * Math.PI * weight * weight);
            double b = ((x * x + y * y) / (2 * weight * weight));
            double result = a * Math.Pow(Math.E, -b);

            return result;
        }

        public static Bitmap convertToBlackAndWhite(Bitmap Bmp) {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++) {
                for (int x = 0; x < Bmp.Width; x++) {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }
            return Bmp;
        }

        private static void dumpMatrix(double[,] values) {
            for (int i = 0; i < values.GetLength(0); i++) {
                Console.Write("[ ");
                for (int k = 0; k < values.GetLength(1); k++) {
                    Console.Write("{0}", values[i, k] + (k + 1 != values.GetLength(1) ? ", " : ""));
                }
                Console.Write(" ]");
                Console.WriteLine();
            }
        }

        public class LockBitmap {
            Bitmap source = null;
            IntPtr Iptr = IntPtr.Zero;
            BitmapData bitmapData = null;

            public byte[] Pixels { get; set; }
            public int Depth { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public LockBitmap(Bitmap source) {
                this.source = source;
            }

            /// <summary>
            /// Lock bitmap data
            /// </summary>
            public void LockBits() {
                try {
                    // Get width and height of bitmap
                    Width = source.Width;
                    Height = source.Height;

                    // get total locked pixels count
                    int PixelCount = Width * Height;

                    // Create rectangle to lock
                    Rectangle rect = new Rectangle(0, 0, Width, Height);

                    // get source bitmap pixel format size
                    Depth = Bitmap.GetPixelFormatSize(source.PixelFormat);

                    // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                    if (Depth != 8 && Depth != 24 && Depth != 32) {
                        throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                    }

                    // Lock bitmap and return bitmap data
                    bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                                 source.PixelFormat);

                    // create byte array to copy pixel values
                    int step = Depth / 8;
                    Pixels = new byte[PixelCount * step];
                    Iptr = bitmapData.Scan0;

                    // Copy data from pointer to array
                    Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
                } catch (Exception ex) {
                    throw ex;
                }
            }

            /// <summary>
            /// Unlock bitmap data
            /// </summary>
            public void UnlockBits() {
                try {
                    // Copy data from byte array to pointer
                    Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                    // Unlock bitmap data
                    source.UnlockBits(bitmapData);
                } catch (Exception ex) {
                    throw ex;
                }
            }

            /// <summary>
            /// Get the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public Color GetPixel(int x, int y) {
                Color clr = Color.Empty;

                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (i > Pixels.Length - cCount)
                    throw new IndexOutOfRangeException();

                if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    byte a = Pixels[i + 3]; // a
                    clr = Color.FromArgb(a, r, g, b);
                }
                if (Depth == 24) // For 24 bpp get Red, Green and Blue
                {
                    byte b = Pixels[i];
                    byte g = Pixels[i + 1];
                    byte r = Pixels[i + 2];
                    clr = Color.FromArgb(r, g, b);
                }
                if (Depth == 8)
                // For 8 bpp get color value (Red, Green and Blue values are the same)
                {
                    byte c = Pixels[i];
                    clr = Color.FromArgb(c, c, c);
                }
                return clr;
            }

            /// <summary>
            /// Set the color of the specified pixel
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="color"></param>
            public void SetPixel(int x, int y, Color color) {
                // Get color components count
                int cCount = Depth / 8;

                // Get start index of the specified pixel
                int i = ((y * Width) + x) * cCount;

                if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                    Pixels[i + 3] = color.A;
                }
                if (Depth == 24) // For 24 bpp set Red, Green and Blue
                {
                    Pixels[i] = color.B;
                    Pixels[i + 1] = color.G;
                    Pixels[i + 2] = color.R;
                }
                if (Depth == 8)
                // For 8 bpp set color value (Red, Green and Blue values are the same)
                {
                    Pixels[i] = color.B;
                }
            }
        }
    }
}