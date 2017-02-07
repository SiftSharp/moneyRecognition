using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApplication1 {
    internal class ImageProccessing {
        private Bitmap img;
        private Bitmap imgBack;
        private LockBitmap bmpBack;
        private LockBitmap bmp;
        public enum sobel{
            Horizontal,
            Vertical,
            DiagonalF,
            DiagonalB
        };
        
        public ImageProccessing(Bitmap img) {
            this.img = img;
            backup();
            bmp = new LockBitmap(img);
        }

        public Bitmap build() {
            return img;
        }

        private void backup() {
            this.imgBack = img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
            bmpBack = new LockBitmap(imgBack);
        }

        public ImageProccessing Sobel() {
            bmp.LockBits();
            backup();
            bmpBack.LockBits();
            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    
                    bmp.SetPixel(x, y,
                        applyConvolutionKernel(
                            generateSobelKernel(sobel.Horizontal),
                            generateSobelKernel(sobel.Vertical),
                            x,
                            y
                       )
                   );
                }
            }
            
            bmpBack.UnlockBits();
            bmp.UnlockBits();
            return this;
        }

        public ImageProccessing Limit(int r, int g, int b) {
            Color pc;
            int tr = 0, tg = 0, tb = 0;
            bmp.LockBits();
            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    pc = bmp.GetPixel(x, y);
                    tr = pc.R < r ? 0 : pc.R;
                    tg = pc.G < r ? 0 : pc.G;
                    tb = pc.B < r ? 0 : pc.B;

                    bmp.SetPixel(x, y, Color.FromArgb(tr,tg,tb));
                }
            }

            bmp.UnlockBits();
            return this;
        }

        public ImageProccessing Gaussian(double weight, int kernelSize) {
            bmp.LockBits();

            // This is our Gaussian Convulated Kernel
            double[,] kernel = generateGaussianKernel(weight, kernelSize);

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

                    color = bmpBack.GetPixel(X, Y);

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
    }
}