using System;
using System.Drawing;

namespace ConsoleApplication1 {
    public class Canny {
        public Bitmap img;
        private int width;
        private int height;
        private int kernelSize = 5;
        float k = 0.04F;
        float threshold = 10000000F;
        private float sigma;
        private float maxHysteresisThresh;
        private float minHysteresisThresh;
        
        private int[,] edgePoints;

        public int[,] greyImage;
        public int[,] edgeMap;
        public int[,] visitedMap;
        public int[,] harriCorners;

        public float[,] filteredImage;
        public float[,] gradient;
        public float[,] nonMax;
        public float[,] GNH;
        public float[,] GNL;
        public float[,] hcr;
        public int[,] hcr2;

        public float[,] derivativeX;
        public float[,] derivativeY;
        public float[,] derivativeXY;

        public Canny(Bitmap img, int width, int height) :this(img,width,height,1.4F, 35F, 10F){}

        public Canny(Bitmap img, int width, int height, float sigma, float maxHys, float minHys) {
            this.img = img;
            this.width = width;
            this.height = height;
            this.sigma = sigma;

            this.maxHysteresisThresh = maxHys;
            this.minHysteresisThresh = minHys;

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
        ///     Resizes a bitmap to a given size and respects ratio
        /// </summary>
        /// <param name="input">A bitmap input image</param>
        /// <param name="maxWidth">Max width of the output image</param>
        /// <param name="maxHeight">Max height of the output image</param>
        /// <returns>Resized image</returns>
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

        private void cannyEdgeDetect() {
            int limit = kernelSize / 2;
            NonMax nonMaxInst = new NonMax();
            // Initialize the different step-arrays
            GNH            = new float[width, height];
            GNL            = new float[width, height];
            edgePoints     = new   int[width, height];
            
            // Filter/blur image
            filteredImage = new Gaussian(3,this.sigma).gaussianFilter(greyImage);
            // NonMax, derivatives and gradient calculations
            nonMax = nonMaxInst.nonMaxSurpress(filteredImage);
            derivativeX = nonMaxInst.derivativeX;
            derivativeY = nonMaxInst.derivativeY;
            derivativeXY = nonMaxInst.derivativeXY;
            // Gradient is calculated in NonMax
            gradient = nonMaxInst.gradient;

            postHysteresis(nonMax);
            hysterisisThresholding(edgePoints);
            harrisCorners();
        }

        private void postHysteresis(float[,] nonMax) {
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

        private void harrisCorners() {
            hcr = new float[width, height];
            harriCorners = new int[width, height];
            NonMax nonMaxInst = new NonMax();
            Gaussian gaussian = new Gaussian();
            derivativeX = gaussian.gaussianFilter(derivativeX);
            derivativeY = gaussian.gaussianFilter(derivativeY);
            derivativeXY = gaussian.gaussianFilter(derivativeXY);

            float val = 0;
            float A, B, C;

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    A = derivativeX[x, y] * derivativeX[x, y];
                    B = derivativeY[x, y] * derivativeY[x, y];
                    C = derivativeXY[x, y];
                    val = ((A * B - (C * C)) - (k * ((A + B) * (A + B))));

                    if (val > threshold) {
                        hcr[x, y] = val;
                    } else {
                        hcr[x, y] = 0;
                    }
                }
            }

            hcr = nonMaxInst.nonMaxSurpress(hcr);
            hcr2 = (int[,]) greyImage.Clone();

            for (int x = 3; x < width-3; x++) {
                for (int y = 3; y < height-3; y++) {

                    if (edgeMap[x, y] > 0) {
                        hcr2[x, y] = 0;
                    }

                    if ((hcr[x,y] > 0 && hcr[x, y] > hcr[x+1, y] && hcr[x, y] > hcr[x - 1, y] && 
                        hcr[x, y] > hcr[x, y - 1] && hcr[x, y] > hcr[x, y + 1] && hcr[x, y] > hcr[x + 1, y + 1] &&
                        hcr[x, y] > hcr[x + 1, y - 1] && hcr[x, y] > hcr[x - 1, y - 1] && hcr[x, y] > hcr[x - 1, y + 1])) {
                        
                        hcr2[x, y] = 255;
                        hcr2[x+1, y] = 255;
                        hcr2[x-1, y] = 255;
                        hcr2[x, y+1] = 255;
                        hcr2[x, y-1] = 255;
                    }
                }
            }
        }
    }
}
