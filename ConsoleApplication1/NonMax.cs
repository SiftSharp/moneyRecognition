using System;

namespace ConsoleApplication1 {
    class NonMax {
        public float[,] gradient;
        private int kernelSize = 5;
        public enum sobel {
            Horizontal,
            Vertical,
            DiagonalF,
            DiagonalB
        };
        public float[,] derivativeX;
        public float[,] derivativeY;
        public float[,] derivativeXY;


        public float[,] nonMaxSurpress(int[,] nonMax) {
            int width = nonMax.GetLength(0), height = nonMax.GetLength(1);
            float[,] nonMaxFloat = new float[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    nonMaxFloat[x, y] = (int)nonMax[x, y];
                }
            }
            return nonMaxSurpress(nonMaxFloat);
        }
        public float[,] nonMaxSurpress(float[,] nonMax) {
            // Generate derivatives of image
            derivativeX = differentiate(nonMax, getSobelKernel(sobel.Horizontal));
            derivativeY = differentiate(nonMax, getSobelKernel(sobel.Vertical));

            return nonMaxSurpress(derivativeX, derivativeY);
        }
        public float[,] nonMaxSurpress(int[,] nonMax, float[,] derivativeX, float[,] derivativeY) {
            return nonMaxSurpress(derivativeX, derivativeY);
        }

        public float[,] nonMaxSurpress(float[,] derivativeX, float[,] derivativeY) {
            float tangent;
            int limit = kernelSize / 2;
            int width = derivativeX.GetLength(0), height = derivativeX.GetLength(1);
            int x, y;
            float[,] nonMax;
            gradient = new float[width, height];
            derivativeXY = new float[width, height];

            // Based on the derivatives from X & Y we can calculated the gradient
            for (x = 0; x < width; x++) {
                for (y = 0; y < height; y++) {
                    gradient[x, y] = hypot(derivativeX[x, y], derivativeY[x, y]);
                    derivativeXY[x,y] = derivativeX[x,y] * derivativeY[x,y];
                }
            }

            nonMax = (float[,])gradient.Clone();
            
            for (x = limit; x < width - limit; x++) {
                for (y = limit; y < height - limit; y++) {
                    if (derivativeX[x, y] == 0) {
                        tangent = 90F;
                    } else {
                        tangent = (float)(Math.Atan2(derivativeY[x, y], derivativeX[x, y]));
                        //tangent = (float) (Math.Atan(Math.Pow(derivativeY[x, y], 2) / Math.Pow(derivativeX[x, y], 2)) / Math.PI);
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
            return nonMax;
        }
        
        private float hypot(float inputA, float inputB) {
            return (float)Math.Sqrt(inputA * inputA + inputB * inputB);
        }

        public float[,] differentiate(int[,] data, double[,] filter) {
            int width = data.GetLength(0), height = data.GetLength(1);
            float[,] dataFloat = new float[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    dataFloat[x, y] = (float)data[x, y];
                }
            }
            return differentiate(dataFloat, filter);
        }
        public float[,] differentiate(float[,] data, double[,] filter) {
            int width = data.GetLength(0), height = data.GetLength(1);

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

        public double[,] getSobelKernel(sobel direction) {
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
    }
}
