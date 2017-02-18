using System;
using System.Drawing;

namespace ConsoleApplication1 {
    public class Canny {
        public Bitmap img;
        private int width;
        private int height;
        private int kernelSize = 5;
        private float maxHysteresisThresh;
        private float minHysteresisThresh;
        private int[,] edgePoints;

        public int[,] greyImage;
        public int[,] filteredImage;
        public int[,] edgeMap;
        public int[,] visitedMap;

        public float[,] derivativeX;
        public float[,] derivativeY;
        public float[,] gradient;
        public float[,] nonMax;
        public float[,] GNH;
        public float[,] GNL;
        public enum sobel {
            Horizontal,
            Vertical,
            DiagonalF,
            DiagonalB
        };

        public Canny(Bitmap img, int width, int height) {
            this.img = img;
            this.width = width;
            this.height = height;

            this.maxHysteresisThresh = 40F;
            this.minHysteresisThresh = maxHysteresisThresh - 5;

            if (height != img.Height || width != img.Width) {
                this.img = resize(this.img, width, height);
                this.width = this.img.Width-1;
                this.height = this.img.Height-1;
            }

            edgeMap = new int[width, height];
            visitedMap = new int[width, height];
            

            this.greyImage = readImage(this.img);
            cannyEdgeDetect();
        }

        public Canny(Bitmap img) :this(img, img.Width, img.Height) { }

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
        
        private int limitVal(int val, int min, int max) {
            val = val > 255 ? 255 : val;
            val = val < 0 ? 0 : val;
            return val;
        }

        /// <summary>
        ///     Generates a Bitmap from 2d array of grayscale pixels
        /// </summary>
        /// <param name="greyImage">2D array of grayscale integers</param>
        /// <returns>Bitmap of given array</returns>
        public Bitmap buildImage(int[,] greyImage) {
            int x, y, val;
            Bitmap output = new Bitmap(this.width, this.height);
            LockBitmap outputLocked = new LockBitmap(output);
            outputLocked.LockBits();

            for (y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    val = limitVal(greyImage[x, y], 0, 255);

                    outputLocked.SetPixel(x, y, Color.FromArgb(
                        val, val, val
                    ));
                }
            }

            outputLocked.UnlockBits();
            return output;
        }

        // Calls the master method to build image from floats
        public Bitmap buildImage(float[,] greyImage) {
            int x, y, width = greyImage.GetLength(0), height = greyImage.GetLength(1);
            int[,] greyImageInt = new int[width,height];
            for (y = 0; y < height; y++) {
                for (x = 0; x < width; x++) {
                    greyImageInt[x, y] = (int)greyImage[x, y];
                }
            }

            return buildImage(greyImageInt);
        }

        // Calls the master method to build image from this.grayImage
        public Bitmap buildImage() {
            return buildImage(greyImage);
        }

        private float hypot(float inputA, float inputB) {
            return (float) Math.Sqrt(inputA * inputA + inputB * inputB);
        }

        private void cannyEdgeDetect() {
            int x, y;
            int limit = kernelSize / 2;

            // Initialize the different step-arrays
            gradient       = new float[width, height];
            nonMax         = new float[width, height];
            derivativeX    = new float[width, height];
            derivativeY    = new float[width, height];
            GNH            = new float[width, height];
            GNL            = new float[width, height];
            edgePoints     = new   int[width, height];

            // Filter/blur image
            filteredImage = gaussianFilter(this.greyImage);

            // Generate derivatives of image
            derivativeX = differentiate(filteredImage, getSobelKernel(sobel.Horizontal));
            derivativeY = differentiate(filteredImage, getSobelKernel(sobel.Vertical));

            // Based on the derivatives from X & Y we can calculated the gradient
            for (x = 0; x < width; x++) {
                for (y = 0; y < height; y++) {
                    gradient[x, y] = hypot(derivativeX[x, y], derivativeY[x, y]);
                }
            }

            // NonMax = Gradient. Copy Gradient into NonMax without ref
            nonMax = (float[,]) gradient.Clone();

            nonMaxSurpress();
            postHysteresis();
            hysterisisThresholding(edgePoints);
        }

        private void postHysteresis() {
            int r, c;
            int limit = kernelSize / 2;
            float min = 100, max = 0;

            for (r = limit; r < width - limit; r++) {
                for (c = limit; c < height - limit; c++) {
                    if ((int)nonMax[r, c] > max) {
                        max = (int)nonMax[r, c];
                    }

                    if (((int)nonMax[r, c] < min) && ((int)nonMax[r, c] > 0)) {
                        min = (int)nonMax[r, c];
                    }
                }
            }

            for (r = limit; r < width - limit; r++) {
                for (c = limit; c < height - limit; c++) {
                    if ((int)nonMax[r, c] >= maxHysteresisThresh) {
                        edgePoints[r, c] = 1;
                        GNH[r, c] = 255;
                    }
                    if (((int)nonMax[r, c] < maxHysteresisThresh) && ((int)nonMax[r, c] >= minHysteresisThresh)) {
                        edgePoints[r, c] = 2;
                        GNL[r, c] = 255;
                    }
                }
            }
        }

        private void nonMaxSurpress() {
            int x, y;
            float tangent;
            int limit = kernelSize / 2;

            for (x = limit; x < width - limit; x++) {
                for (y = limit; y < height - limit; y++) {
                    if (derivativeX[x, y] == 0) {
                        tangent = 90F;
                    } else {
                        tangent = (float)(Math.Atan2(derivativeY[x, y], derivativeX[x, y]));
                    }
                    //Horizontal Edge
                    if (((-22.5 < tangent) && (tangent <= 22.5)) || ((157.5 < tangent) && (tangent <= -157.5))) {
                        if ((gradient[x, y] < gradient[x, y + 1]) || (gradient[x, y] < gradient[x, y - 1]))
                            nonMax[x, y] = 0;
                    }
                    //Vertical Edge
                    if (((-112.5 < tangent) && (tangent <= -67.5)) || ((67.5 < tangent) && (tangent <= 112.5))) {
                        if ((gradient[x, y] < gradient[x + 1, y]) || (gradient[x, y] < gradient[x - 1, y]))
                            nonMax[x, y] = 0;
                    }
                    //+45 Degree Edge
                    if (((-67.5 < tangent) && (tangent <= -22.5)) || ((112.5 < tangent) && (tangent <= 157.5))) {
                        if ((gradient[x, y] < gradient[x + 1, y - 1]) || (gradient[x, y] < gradient[x - 1, y + 1]))
                            nonMax[x, y] = 0;
                    }
                    //-45 Degree Edge
                    if (((-157.5 < tangent) && (tangent <= -112.5)) || ((67.5 < tangent) && (tangent <= 22.5))) {
                        if ((gradient[x, y] < gradient[x + 1, y + 1]) || (gradient[x, y] < gradient[x - 1, y - 1]))
                            nonMax[x, y] = 0;
                    }
                }
            }
        }


        private void hysterisisThresholding(int[,] edges) {
            
            int x, y;
            int limit = kernelSize / 2;

            for (x = limit; x < width - limit; x++) {
                for (y = limit; y <= (height - 1) - limit; y++) {
                    if (edges[x, y] == 1) {
                        edgeMap[x, y] = 1;
                    }
                }
            }

            for (x = limit; x < width - limit; x++) {
                for (y = limit; y < height - limit; y++) {
                    if (edges[x, y] == 1) {
                        edgeMap[x, y] = 1;
                        travers(x, y);
                        visitedMap[x, y] = 1;
                    }
                }
            }

            for (x = 0; x < width; x++) {
                for (y = 0; y < height; y++) {
                    edgeMap[x, y] = edgeMap[x, y] * 255;
                }
            }
        }

        private void travers(int X, int Y) {
            if (visitedMap[X, Y] == 1) {
                return;
            }
            //1
            if (edgePoints[X + 1, Y] == 2) {
                edgeMap[X + 1, Y] = 1;
                visitedMap[X + 1, Y] = 1;
                travers(X + 1, Y);
                return;
            }
            //2
            if (edgePoints[X + 1, Y - 1] == 2) {
                edgeMap[X + 1, Y - 1] = 1;
                visitedMap[X + 1, Y - 1] = 1;
                travers(X + 1, Y - 1);
                return;
            }
            //3
            if (edgePoints[X, Y - 1] == 2) {
                edgeMap[X, Y - 1] = 1;
                visitedMap[X, Y - 1] = 1;
                travers(X, Y - 1);
                return;
            }
            //4
            if (edgePoints[X - 1, Y - 1] == 2) {
                edgeMap[X - 1, Y - 1] = 1;
                visitedMap[X - 1, Y - 1] = 1;
                travers(X - 1, Y - 1);
                return;
            }
            //5
            if (edgePoints[X - 1, Y] == 2) {
                edgeMap[X - 1, Y] = 1;
                visitedMap[X - 1, Y] = 1;
                travers(X - 1, Y);
                return;
            }
            //6
            if (edgePoints[X - 1, Y + 1] == 2) {
                edgeMap[X - 1, Y + 1] = 1;
                visitedMap[X - 1, Y + 1] = 1;
                travers(X - 1, Y + 1);
                return;
            }
            //7
            if (edgePoints[X, Y + 1] == 2) {
                edgeMap[X, Y + 1] = 1;
                visitedMap[X, Y + 1] = 1;
                travers(X, Y + 1);
                return;
            }
            //8
            if (edgePoints[X + 1, Y + 1] == 2) {
                edgeMap[X + 1, Y + 1] = 1;
                visitedMap[X + 1, Y + 1] = 1;
                travers(X + 1, Y + 1);
                return;
            }
            
            visitedMap[X, Y] = 1;
        }


        private int[,] gaussianFilter(int[,] data) {
            int[,] output = new int[width, height];
            int size = 2;
            float sigma = 10.4F;
            int i, j, k, l; // for variables
            float sum = 0;

            // Generate 
            double[,] gaussianKernel = generateGaussianKernel(sigma, size);
            int limit = gaussianKernel.GetLength(0) / 2;

            // Copy values for persistant read
            output = data; 

            for (i = limit; i < width - limit; i++) {
                for (j = limit; j < height - limit; j++) {
                    sum = 0;
                    for (k = -limit; k <= limit; k++) {
                        for (l = -limit; l <= limit; l++) {
                            sum = sum + (data[i + k, j + l] * (float)gaussianKernel[limit + k, limit + l]);
                        }
                    }
                    output[i, j] = (int) sum;
                }
            }

            return output;
        }

        private float[,] differentiate(int[,] data, double[,] filter) {
            int i, j, k, l, filterHeigt, filterWidth;

            filterWidth = filter.GetLength(0);
            filterHeigt = filter.GetLength(1);
            float sum = 0;
            float[,] output = new float[width, height];

            for (i = filterWidth / 2; i < width - filterWidth / 2; i++) {
                for (j = filterHeigt / 2; j < height - filterHeigt / 2; j++) {
                    sum = 0;
                    for (k = -filterWidth / 2; k <= filterWidth / 2; k++) {
                        for (l = -filterHeigt / 2; l <= filterHeigt / 2; l++) {
                            sum = sum + data[i + k, j + l] * (float)filter[filterWidth / 2 + k, filterHeigt / 2 + l];
                        }
                    }
                    output[i, j] = sum;
                }
            }
            return output;
        }

        public Bitmap resize(Bitmap input, int maxWidth, int maxHeight) {

            double ratioX = (double)maxWidth / input.Width;
            double ratioY = (double)maxHeight / input.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(input.Width * ratio);
            int newHeight = (int)(input.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage)) {
                graphics.DrawImage(input, 0, 0, newWidth, newHeight);
            }

            input = newImage.Clone(new Rectangle(0, 0, newImage.Width, newImage.Height), newImage.PixelFormat);

            return input;
        }

        private double[,] generateGaussianKernel(double weight, int size) {
            size = 2 * size + 1;
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

        private double[,] getSobelKernel(sobel direction) {
            switch (direction) {
                case sobel.Vertical:
                    return new double[,] { 
                        { -1, 0, 1 }, 
                        { -2, 0, 2 }, 
                        { -1, 0, 1 }
                    };
                case sobel.Horizontal:
                    return new double[,] { 
                        {  1,  2,  1 }, 
                        {  0,  0,  0 }, 
                        { -1, -2, -1 }
                    };
                case sobel.DiagonalF:
                    return new double[,] { 
                        { -2, -1,  0 }, 
                        { -1,  0,  1 }, 
                        {  0,  1,  2 }
                    };
                case sobel.DiagonalB:
                    return new double[,] { 
                        {  0, -1, -2 }, 
                        {  1,  0, -1 }, 
                        {  2,  1,  0 }
                    };
                default:
                    return null;
            }
        }

        private double calculateKernelEntity(int x, int y, double weight) {
            // (1 / (2 * pi * (weight) ^ 2)) * e ^ (-((x ^ 2 + y ^ 2) / (2 * (weight) ^ 2)))
            double a = 1 / (2 * Math.PI * weight * weight);
            double b = ((x * x + y * y) / (2 * weight * weight));
            double result = a * Math.Pow(Math.E, -b);
            return result;
        }
    }
}
