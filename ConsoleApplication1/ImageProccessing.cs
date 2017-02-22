using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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
        private double[,][][] convulutionValues;


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

        public ImageProccessing resize(int maxWidth, int maxHeight) {
            
            double ratioX = (double)maxWidth / img.Width;
            double ratioY = (double)maxHeight / img.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            img = newImage.Clone(new Rectangle(0, 0, newImage.Width, newImage.Height), newImage.PixelFormat);
            bmp = new LockBitmap(img);
            backup();
            return this;
        }


        public ImageProccessing SobelSupression() {
            backup();
            bmpBack.LockBits();
            bmp.LockBits();
            int height = img.Height;
            int width = img.Width;
            double[][,] kernelsToApply = new double[][,]{
                generateSobelKernel(sobel.Vertical),
                generateSobelKernel(sobel.Horizontal)
            };
            convulutionValues = new double[bmp.Width, bmp.Height][][];

            Parallel.For(0, bmp.Height * bmp.Width, (q, state) => {
                int y = q / width;
                int x = q - y * width;
                
                convulutionValues[x, y] = applyConvolutionKernels(
                    kernelsToApply, x, y, true
                );

                double[] limtedValues = sumSqrt(convulutionValues[x, y]);

                for (int i = 0; i < limtedValues.GetLength(0); i++) {
                    limtedValues[i] = limtedValues[i] > 255 ? 255 : limtedValues[i];
                    limtedValues[i] = limtedValues[i] <   0 ?   0 : limtedValues[i];
                }

                bmp.SetPixel(x, y,
                    Color.FromArgb((int)limtedValues[0], (int)limtedValues[0], (int)limtedValues[0])
               );
            });


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

                    bmp.SetPixel(x, y, Color.FromArgb(tr,tr,tr));
                }
            }

            bmp.UnlockBits();
            return this;
        }

        public ImageProccessing Gaussian(double weight, int kernelSize) {
            bmp.LockBits(); 
            backup();
            bmpBack.LockBits();
            

            // This is our Gaussian Convulated Kernel
            double[,] kernel = generateGaussianKernel(weight, kernelSize);
            int height = img.Height;
            int width = img.Width;
            Parallel.For(0, bmp.Height * bmp.Width, (q, state) => {
                int y = q / width;
                int x = q - y * width;

                Color color;
                double[] sum = applyConvolutionKernels(new double[][,] { kernel }, x, y);

                for (int i = 0; i < sum.GetLength(0); i++) {
                    sum[i] = sum[i] > 255 ? 255 : sum[i];
                    sum[i] = sum[i] < 0 ? 0 : sum[i];
                }

                color = Color.FromArgb((int)sum[0], (int)sum[0], (int)sum[0]);

                bmp.SetPixel(x, y, color);
            });

            bmpBack.UnlockBits();
            bmp.UnlockBits();
            return this;
        }

        public ImageProccessing nonMaximumSurrpression() {
                        bmp.LockBits();

            int height = img.Height;
            int width = img.Width;
            double[,] Gradient = new double[width, height];
            double[,] NonMax = new double[width, height];
            int[,] PostHysteresis = new int[width, height];

            for (int Y = 0; Y < img.Height; Y++) {
                for (int X = 0; X < img.Width; X++) {
                    Gradient[X, Y] = (float)Math.Sqrt(Math.Pow(convulutionValues[X, Y][0][0],2) + Math.Pow(convulutionValues[X, Y][1][0], 2));
                    NonMax[X, Y] = Gradient[X, Y];
                }
            }


            /*for (int Y = 0; Y < img.Height; Y++) {
                for (int X = 0; X < img.Width; X++) {
                    Gradient[X, Y] = sumSqrt(convulutionValues[X, Y])[0] / 180;
                    NonMax[X, Y] = Gradient[X, Y];
                }
            }*/

            
            int r, c;
            int Limit = 3/2;


            Parallel.For(0, bmp.Height * bmp.Width, (q, state) => {
                int Y = q / width;
                int X = q - Y * width;
                float tangent = 0.0F;
                // Just drop values outside the image (you could do something else)
                if (Y - 1 < 0 || X - 1 < 0 || Y + 2 > bmp.Height || X + 2 > bmp.Width) {
                    return;
                }

                if (convulutionValues[X, Y][0][0] == 0) {
                    tangent = 90F;
                }
                else {
                    // converting radians to degrees
                    tangent = (float)
                        (Math.Atan(
                            Math.Pow(convulutionValues[X, Y][0][0],2) / 
                            Math.Pow(convulutionValues[X, Y][1][0],2)
                        ) * 180 / Math.PI
                    );
                    //tangent = (float)
                    //    (Math.Atan2(convulutionValues[X, Y][0][0], convulutionValues[X, Y][1][0]));
                }

                //Console.WriteLine("{0}: {1},{2}",tangent, convulutionValues[X, Y][0][0], convulutionValues[X, Y][1][0]);

                //Horizontal Edge
                if (((-22.5 < tangent) && (tangent <= 22.5)) || ((157.5 < tangent) && (tangent <= -157.5))) {
                    if ((Gradient[X, Y] < Gradient[X, Y + 1]) || (Gradient[X, Y] < Gradient[X, Y - 1]))
                        NonMax[X, Y] = 0;
                }

                //Vertical Edge
                if (((-112.5 < tangent) && (tangent <= -67.5)) || ((67.5 < tangent) && (tangent <= 112.5))) {
                    if ((Gradient[X, Y] < Gradient[X + 1, Y]) || (Gradient[X, Y] < Gradient[X - 1, Y]))
                        NonMax[X, Y] = 0;
                }

                //+45 Degree Edge
                if (((-67.5 < tangent) && (tangent <= -22.5)) || ((112.5 < tangent) && (tangent <= 157.5))) {
                    if ((Gradient[X, Y] < Gradient[X + 1, Y - 1]) || (Gradient[X, Y] < Gradient[X - 1, Y + 1]))
                        NonMax[X, Y] = 0;
                }

                //-45 Degree Edge
                if (((-157.5 < tangent) && (tangent <= -112.5)) || ((67.5 < tangent) && (tangent <= 22.5))) {
                    if ((Gradient[X, Y] < Gradient[X + 1, Y + 1]) || (Gradient[X, Y] < Gradient[X - 1, Y - 1]))
                        NonMax[X, Y] = 0;
                }

                double color = NonMax[X, Y] > 255 ? 255 : NonMax[X, Y] < 0 ? 0 : NonMax[X, Y];
    
                bmp.SetPixel(X, Y, Color.FromArgb((int)color, (int)color, (int)color));                
            });

            //PostHysteresis = NonMax;
            for (r = Limit; r <= (width - Limit) - 1; r++) {
                for (c = Limit; c <= (height - Limit) - 1; c++) {

                    PostHysteresis[r, c] = (int)NonMax[r, c];
                }

            }


            bmp.UnlockBits();
            return this;
        }

        private bool checkNeighbourPixels(LockBitmap bmp, int x, int y, int value) {
            return bmp.GetPixel(x, y + 1).B >= value || bmp.GetPixel(x, y - 1).B >= value ||
                   bmp.GetPixel(x + 1, y).B >= value || bmp.GetPixel(x - 1, y).B >= value;
        }

        private double[] applyConvolutionKernels(double[][,] kernels, int x, int y) {
            double[][] sumsFromKernels = applyConvolutionKernels(kernels, x, y, true);
            return sumSqrt(sumsFromKernels);
        }

        private double[] sumSqrt(double[][] sums) {
            double[] final = new double[3];
            for (int i = 0; i < sums.GetLength(0); i++) {
                sums[i] = sums[i].Select(el => el * el).ToArray();
            }

            for (int i = 0; i < sums.GetLength(0); i++) {
                for (int j = 0; j < 3; j++) {
                    final[j] += sums[i][j];
                }
            }

            final = final.Select(el => Math.Sqrt(el)).ToArray();

            return final;
        }

        private double[][] applyConvolutionKernels(double[][,] kernels, int x, int y, bool returnRaw) {
            // The backup image and the actual image should be same dimensions
                                    
            int kernelRadius = kernels[0].GetLength(0) / 2;
            Color color;
            double[][] sumsFromKernels = new double[kernels.GetLength(0)][];
            int amountOfActivePixels = 0;

            sumsFromKernels = sumsFromKernels.Select(
                el => new double[3] { 0.0, 0.0, 0.0 }
            ).ToArray();

            for (int Y = (y - kernelRadius); Y <= (y + kernelRadius); Y++) {
                for (int X = (x - kernelRadius); X <= (x + kernelRadius); X++) {

                    // Just drop values outside the image (you could do something else)
                    if (Y < 0 || X < 0 || Y + 1 > bmp.Height || X + 1 > bmp.Width) {
                        continue;
                    }

                    for(int i=0; i<kernels.GetLength(0);i++) {
                        double[] sumX = new double[kernels[i].GetLength(0)];
                        double[] sumY = new double[kernels[i].GetLength(1)];
                        color = bmpBack.GetPixel(X, Y);
                        
                        sumsFromKernels[i][0] += color.R * kernels[i][X - (x - kernelRadius), Y - (y - kernelRadius)];
                        //sumsFromKernels[i][1] += color.G * kernels[i][X - (x - kernelRadius), Y - (y - kernelRadius)];
                        //sumsFromKernels[i][2] += color.B * kernels[i][X - (x - kernelRadius), Y - (y - kernelRadius)];
                    }

                    amountOfActivePixels++;
                }
            }

            return sumsFromKernels;
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

        public ImageProccessing convertToBlackAndWhite() {
            int rgb;
            Color c;

            bmp.LockBits();

            for (int y = 0; y < bmp.Height; y++) {
                for (int x = 0; x < bmp.Width; x++) {
                    c = bmp.GetPixel(x, y);
                    rgb = ((c.R + c.G + c.B) / 3);
                    bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }

            bmp.UnlockBits();
            backup();
            return this;
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