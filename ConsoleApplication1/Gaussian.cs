using System;

namespace ConsoleApplication1 {
    class Gaussian {
        private int size;
        private float sigma;

        public Gaussian(int kernelSize, float sigma) {
            this.size = kernelSize;
            this.sigma = sigma;
        }

        public Gaussian(){
            this.size = 3;
            this.sigma = 1.4F;
        }

        public float[,] gaussianFilter(int[,] data) {
            
            int width = data.GetLength(0), height = data.GetLength(1);
            float[,] dataFloat = new float[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    dataFloat[x, y] = (int)data[x, y];
                }
            }
            return gaussianFilter(dataFloat);   
        }

        public float[,] gaussianFilter(float[,] data) {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            float[,] output = new float[width, height];
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
                    output[i, j] = (int)sum;
                }
            }

            return output;
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


        private double calculateKernelEntity(int x, int y, double weight) {
            // (1 / (2 * pi * (weight) ^ 2)) * e ^ (-((x ^ 2 + y ^ 2) / (2 * (weight) ^ 2)))
            double a = 1 / (2 * Math.PI * weight * weight);
            double b = ((x * x + y * y) / (2 * weight * weight));
            double result = a * Math.Pow(Math.E, -b);
            return result;
        }
    }
}
