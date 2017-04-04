using System;
using System.Linq;
using System.Drawing;
using System.IO;

namespace SiftSharp {
    public class Image : ICloneable {
        const int GetWidth = 0;
        const int GetHeight = 1;
        private float[,] img;

        public Image(float[,] img)
        {
            this.img = img;
        }

        public Image(string path) {
            Bitmap bitmapInput;
            try {
                bitmapInput = new Bitmap(path);
            }
            catch (Exception ex) {
                throw new InvalidDataException(
                    "Either no file is at "+path+" or Image was 'null' and cannot continue...");
            }
            img = ReadImage(bitmapInput);
        }

        /// <summary>
        ///     Generates a Bitmap from img
        /// </summary>
        /// <returns>Bitmap of img float array</returns>
        public Bitmap AsBitmap()
        {
            return BuildImage(img);
        }

        /// <summary>
        /// Gets the private img array
        /// </summary>
        /// <returns>Float array representing input image</returns>
        public float[,] Get()
        {
            return img;
        }

        /// <summary>
        /// Creates a clone of this instance
        /// </summary>
        /// <returns>A clone of this image</returns>
        public Image Clone()
        {
            return (Image)this.MemberwiseClone();
        }

        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="width">The desired width</param>
        /// <param name="height">The desired height</param>
        /// <returns>A resized instance</returns>
        public Image Resize(int width, int height)
        {
            this.img = Resize(width, height, this.img);
            return this;
        }

        /// <summary>
        /// Downsamples the image by deleting every second row and col.
        /// </summary>
        /// <returns>A downsampled instance</returns>
        public Image Downsample()
        {
            this.img = Downsample(this.img);
            return this;
        }

        /// <summary>
        /// Resizes image to maximal size.
        /// </summary>
        /// <param name="max_size">The maximal size of image</param>
        /// <returns>A resized image instance</returns>
        public Image Maxsize(int max_size)
        {
            this.img = Maxsize(max_size, this.img);
            return this;
        }

        /// <summary>
        /// Interface for ICloneable
        /// </summary>
        /// <returns>A clone of this image</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }


        public void resize() { }
        public void scale() { }


        //SlideTypes: Flag used for SlidingWindow()
        //Used to specifiy whether Convolution or Cross Correlation should be applied when using SlidingWindow()
        [Flags]
        public enum SlideTypes
        {
            None,
            Convolution,
            CrossCorrelation
        }

        /// <summary>
        /// This is a helper method for overloading params to Guassian funcion.
        /// This means that if you only provide a sigma value, then the
        /// Guassian call will be of a kernel size on 3 and the
        /// object data-stream.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <returns>Return gaussian blurred image as 2d float array</returns>
        public Image Gaussian(float sigma)
        {
            this.img = Gaussian(sigma, 3, this.img); // Calls the Gaussian method with 3 inputs.
            return this;
        }

        /// <summary>
        /// This is a helper method for overloading params to Guassian funcion.
        /// This means that if you only provide a sigma value, and a kernel
        /// size then the Guassian call will be of the object data-stream.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <param name="size">The kernel size</param>
        /// <returns>Return gaussian blurred image as 2d float array</returns>
        public Image Gaussian(float sigma, int size)
        {
            this.img = Gaussian(sigma, size, this.img); // Calls the Gaussian method with 3 inputs.
            return this;
        }

        /// <summary>
        /// Sobel Kernels
        /// </summary>
        /// <returns>A jaggered array with the possibility to choose between different matrices</returns>
        public float[][,] Sobel(int size)
        {

            int[][,] SobelKernels = new int[2][,];

            switch (size)
            {
                case 3:
                    // Sobel 3x3 Kernel x
                    SobelKernels[0] = new int[,]{
                        { -1, 0, 1 },
                        { -2, 0, 2 },
                        { -1, 0, 1 } };

                    // Sobel 3x3 Kernel y
                    SobelKernels[1] = new int[,]{
                        { 1, 2, 1 },
                        { 0, 0, 0 },
                        { -1, -2, -1 } };
                    break;
                case 5:
                    // Sobel 5x5 Kernel x
                    SobelKernels[0] = new int[,]{
                        { -2, -1, 0, 1, 2 },
                        { -3, -2, 0, 2, 3 },
                        { -4, -3, 0, 3, 4 },
                        { -3, -2, 0, 2, 3 },
                        { -2, -1, 0, 1, 2 }};

                    // Sobel 5x5 Kernel y
                    SobelKernels[1] = new int[,]{
                        { 2, 3, 4, 3, 2 },
                        { 1, 2, 3, 2, 1 },
                        { 0, 0, 0, 0, 0 },
                        { -1, -2, -3, -2, -1 },
                        { -2, -3, -4, -3, -2 }};
                    break;
                case 7:
                    // Sobel 7x7 Kernel x
                    SobelKernels[0] = new int[,] {
                        { -3, -2, -1, 0, 1, 2, 3 },
                        { -4, -3, -2, 0, 2, 3, 4 },
                        { -5, -4, -3, 0, 3, 4, 5 },
                        { -6, -5, -4, 0, 4, 5, 6 },
                        { -5, -4, -3, 0, 3, 4, 5 },
                        { -4, -3, -2, 0, 2, 3, 4 },
                        { -3, -2, -1, 0, 1, 2, 3 }};

                    // Sobel 7x7 Kernel y
                    SobelKernels[1] = new int[,] {
                       { 3, 4, 5, 6, 5, 4, 3 },
                       { 2, 3, 4, 5, 4, 3, 2 },
                       { 1, 2, 3, 4, 3, 2, 1 },
                       { 0, 0, 0, 0, 0, 0, 0 },
                       { -1, -2, -3, -4, -3, -2, -1 },
                       { -2, -3, -4, -5, -4, -3, -2 },
                       { -3, -4, -5, -6, -5, -4, -3 }};
                    break;
                default:
                    throw new InvalidDataException("Size can only be 3, 5 or 7");
            }
            return SlidingWindow(img, SobelKernels, SlideTypes.Convolution);
        }

        public Image Convolve<T>(T[,] kernel)
        {
            this.img = SlidingWindow(this.img, new T[][,] { kernel }, SlideTypes.Convolution)[0];
            return this;
        }

        public Image CrossCorrelate<T>(T[,] kernel)
        {
            this.img = SlidingWindow(this.img, new T[][,] { kernel }, SlideTypes.CrossCorrelation)[0];
            return this;
        }










        /* STATIC METHODS */





        /// <summary>
        ///     Reads an input bitmap and generates
        ///     a B&W data output
        /// </summary>
        /// <param name="input">a bitmap image</param>
        /// <returns>Black and white two dimensional array of image</returns>
        public static float[,] ReadImage(Bitmap input)
        {
            int width = input.Width,
                height = input.Height;
            float[,] output = new float[width, height];
            Color colors;
            LockBitmap inputLocked = new LockBitmap(input);
            inputLocked.LockBits();

            // Store grayscale value for each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors = inputLocked.GetPixel(x, y);
                    output[x, y] = (int)(
                        (colors.R + colors.G + colors.B) / 3.0F
                    );
                }
            }

            inputLocked.UnlockBits();

            return output;
        }

        /// <summary>
        ///     Generates a Bitmap from 2d array of grayscale pixels
        /// </summary>
        /// <param name="greyImage">2D array of grayscale values</param>
        /// <returns>Bitmap of given array</returns>
        public static Bitmap BuildImage<T>(T[,] greyImage)
        {
            int Width = greyImage.GetLength(0),
                Height = greyImage.GetLength(1);

            // Create bitmap with input dimensions
            Bitmap output = new Bitmap(Width, Height);

            // Lock bitmap
            LockBitmap outputLocked = new LockBitmap(output);
            outputLocked.LockBits();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Truncate pixel values
                    int val = (int)LimitToValues(greyImage[x, y], 0, 255);
                    if(val < 0)
                    {
                        val = 0;
                    }

                    // Save pixel value
                    outputLocked.SetPixel(x, y, Color.FromArgb(
                        val, val, val
                    ));
                }
            }

            // Unlock bitmap
            outputLocked.UnlockBits();

            return output;
        }

        /// <summary>
        /// Takes a value and checks if it is over or below thresholds.
        /// If so, it truncates the value to either max or min.
        /// </summary>
        /// <typeparam name="T">Double, float or int</typeparam>
        /// <param name="val">Value to be checked</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Truncated value</returns>
        private static float LimitToValues<T>(T val, int min, int max)
        {
            // Parse number to float
            float ParsedValue = checked(float.Parse(val.ToString()));

            // Over
            if (ParsedValue > max)
            {
                return max;
            }
            // Below
            else if (ParsedValue < min)
            {
                return min;
            }
            // Neither over or below
            return ParsedValue;
        }

        /// <summary>
        /// This is the main Gaussian method, where all params are required.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <param name="size">The kernel size</param>
        /// <param name="stream">The data-stream of the image</param>
        /// <returns>Return gaussian blurred image as 2d float array</returns>
        public static float[,] Gaussian<T>(float sigma, int size, T[,] stream)
        {
            float[,] kernel = GenerateGuassianKernel(sigma, size);

            // Since the slidingWindow method needs an array of kernels, we gotta save our kernel in an array first.
            // Meaby this could be changed in SlidingWindow ?
            float[][,] kernels = { kernel };

            // Now we send the data-stream + the kernel to the convolution operation and return its value.
            return SlidingWindow(stream, kernels, SlideTypes.Convolution)[0];

        }

        /// <summary>
        /// Generates a Gaussian-kernel.
        /// </summary>
        /// <param name="sigma">The desired sigma value</param>
        /// <param name="size">The kernel size</param>
        /// <returns>Return kernel as 2d float array</returns>
        /// <exception cref="InvalidDataException">Checks if size is odd, in order to do correct gaussian</exception>
        public static float[,] GenerateGuassianKernel(float sigma, int size)
        {
            if (size % 2 == 0)
            {
                throw new InvalidDataException("Has to be odd size, inorder to have a center-center point.");
            }
            float[,] kernel = new float[size, size]; // Creates a new 2d array for the kernel (The matrix of the kernel)
            int kernelRadius = size / 2; // Sets the radius, this is needed since the middel of the kernel is cosidered [0,0]

            // The gaussian function is (1 / (2*PI*sigma^2))*e^(-1*(x^2+y^2)/(2*sigma^2))
            // For different purposes this equation is substracted into sub-parts so that,
            float c = 1 / (2 * (float)Math.PI * (sigma * sigma)); // (1 / (2*PI*sigma^2)) is substituded with c.
            float k = 2 * sigma * sigma; // (2*sigma^2) is substituded with k.

            float accumulatedSum = 0.0f; // This is the accumulated sum, which is used to normalize the data later on.

            for (int y = -kernelRadius; y <= kernelRadius; y++) // loop through the kernel from bottom row to top
            {
                for (int x = -kernelRadius; x <= kernelRadius; x++) // loop through the columns from right to left
                {
                    float value = c * (float)Math.Exp(-1 * ((y * y + x * x) / (k))); // The new equation is c*e^(-1*(x^2+y^2)/k).
                    accumulatedSum += value; // adds the value to the accumulated sum.
                    kernel[y + kernelRadius, x + kernelRadius] = value; // stores the value in the kernel-matrix.
                }
            }

            kernel = NormalizeKernel(kernel, accumulatedSum, size); // Normalize the kernel.

            return kernel;
        }

        /// <summary>
        /// This function normalizes any kernel.
        /// </summary>
        /// <param name="kernel">The kernel that needs to be normalized</param>
        /// <param name="accumulatedSum">The accumulated sum of the kernel.</param>
        /// <param name="size">The kernel size</param>
        /// <returns>The kernel normalized</returns>
        public static float[,] NormalizeKernel(float[,] kernel, float accumulatedSum, int size)
        {
            // Here we loop through the kernel in order to normalize all the data.
            for (int y = 0; y < size; y++) // Loops through the rows top to bottom.
            {
                for (int x = 0; x < size; x++) // Loops through the columns left to right.
                {
                    kernel[y, x] = kernel[y, x] * (1.0f / accumulatedSum); //Normalizes the data by deviding it with the accumulated sum.
                }
            }
            return kernel;
        }

        /// <summary>
        /// Performs Convolution or Cross correlation on image with user specified kernels
        /// </summary>
        /// <param name="image" type="double[,]">Image matrix</param>
        /// <param name="kernels" type="T[][,]">Array of kernels</param>
        /// <param name="slideType" type="enum">Convolution or Crosscorrelation flag</param>
        /// <returns>New image after filter applied</returns>
        public static float[][,] SlidingWindow<I, T>(I[,] image, T[][,] kernels, SlideTypes slideType)
        {
            int imageWidth = image.GetLength(0), imageHeight = image.GetLength(1);
            int kernelSize = kernels[0].GetLength(0);
            int numberOfKernels = kernels.GetLength(0);
            int kernelCenter = kernelSize / 2;
            float[,] imageAsFloat = AsFloat(image);

            //Initialize array of Floats
            float[][,] kernelsAsFloats = Enumerable
                .Range(0, numberOfKernels)
                .Select(_ => new float[kernelSize, kernelSize])
                .ToArray();

            //Initialize array of 2D arrays with correct size
            float[][,] result = Enumerable
                .Range(0, numberOfKernels)//From 0 to the number of kernels
                .Select(_ => new float[imageWidth, imageHeight])//Create new float
                .ToArray();//Make it an array

            //Make kernels float[,] from generic
            for (int i = 0; i < numberOfKernels; i++)
            {
                kernelsAsFloats[i] = AsFloat(kernels[i]);
            }

            //Loops through image pixels
            for (int y = kernelCenter; y < (imageHeight - kernelCenter); y++)
            {
                for (int x = kernelCenter; x < (imageWidth - kernelCenter); x++)
                {
                    //Loops through all kernels in kernels[] and calls ApplyKernel() with said kernel
                    for (int k = 0; k < numberOfKernels; k++)
                    {
                        result[k][x, y] = ApplyKernel(imageAsFloat, kernelsAsFloats[k], x, y, slideType);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Helper: Convert multidimensional generic to float[,]
        /// </summary>
        /// <param name="genericArray" type="T[,]">Generic matrix</param>
        /// <returns>returns float[,]</returns>
        public static float[,] AsFloat<T>(T[,] genericArray)
        {
            int arrayWidth = genericArray.GetLength(0), arrayHeight = genericArray.GetLength(1);
            float[,] arrayAsFloat = new float[arrayWidth, arrayHeight];
            //Loop through each pixel in array from left to right
            for (int y = 0; y < arrayHeight; y++)
            {
                for (int x = 0; x < arrayWidth; x++)
                {
                    //Convert generic type to string and thereafter parse the string to float
                    arrayAsFloat[x, y] = float.Parse(genericArray[x, y].ToString());
                }
            }
            return arrayAsFloat;
        }

        /// <summary>
        /// Calculates sum of filtered neighborhood and returns new convolved or cross correlated pixel
        /// </summary>
        /// <param name="image" type="float[,]">Image matrix</param>
        /// <param name="kernel" type="float[,]">Kernel</param>
        /// <param name="x" type="int">pixel x coordinate from image</param>
        /// <param name="y" type="int">pixel y coordinate from image</param>
        /// <param name="slideType" type="enum">Convolution or Crosscorrelation flag</param>
        /// <returns>A pixel after summation of local neighborhood</returns>
        public static float ApplyKernel(float[,] image, float[,] kernel, int x, int y, SlideTypes slideType)
        {
            int kernelSize = kernel.GetLength(0);
            int kernelCenter = kernelSize / 2;
            SlideTypes flags = SlideTypes.Convolution | SlideTypes.CrossCorrelation;
            float sum = 0.0F;

            //Loop through kernel from negative index away from center to positive index away from center
            for (int kernelY = -kernelCenter; kernelY <= kernelCenter; kernelY++)
            {
                for (int kernelX = -kernelCenter; kernelX <= kernelCenter; kernelX++)
                {
                    if ((flags & SlideTypes.Convolution) == SlideTypes.Convolution)
                    {
                        //Convolution
                        sum += (image[x + kernelX, y + kernelY] *
                                kernel[kernelCenter - kernelX, kernelCenter - kernelY]);
                    }
                    else if ((flags & SlideTypes.CrossCorrelation) == SlideTypes.CrossCorrelation)
                    {
                        //Cross Correlation
                        sum += (image[x - kernelX, y - kernelY] *
                                kernel[kernelCenter + kernelX, kernelCenter + kernelY]);
                    }
                    else throw new Exception("Please provide the type of window slide");
                }
            }
            return sum;
        }

        public static float[,] Resize(int width, int height, float[,] img)
        {
            try
            {
                Bitmap b = new Bitmap(width, height); // Create a new bitmap.
                Bitmap source = BuildImage(img);
                using (Graphics g = Graphics.FromImage(b))  // Graphics object from bitmap.
                {
                    // sets interpolationmode to bicubic.
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    // interpolate the image.
                    g.DrawImage(source, 0, 0, width, height);
                }
                return ReadImage(b); // returning the resized bitmap.
            }
            catch  // Errorhandeling here.
            {
                throw new InvalidOperationException("Bitmap could not be resized"); // Error msg.
            }
        }

        /* Removes every second pixel from image array. Render image half size.*/
        public static float[,] Downsample(float[,] img)
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

        /*Sets the maximal size of image.*/
        public static float[,] Maxsize(int max_size, float[,] image)
        {
            double scale;
            double new_height;
            double new_width;
            int width = image.GetLength(GetWidth);
            int height = image.GetLength(GetHeight);

            if (max_size == width || max_size == height)
                return image;

            if (width == height)
            {
                scale = max_size / Convert.ToDouble (width);
                new_height = height * scale;
                new_width = new_height;
                return Resize(Convert.ToInt32 (new_width), Convert.ToInt32 (new_height), image);
            }
            else if (width > height)
            {
                scale = max_size / Convert.ToDouble(width);
                new_height = height * scale;
                new_width = max_size;
                return Resize(Convert.ToInt32 (new_width), Convert.ToInt32 (new_height), image);
            }
            else if (width < height)
            {
                scale = max_size / Convert.ToDouble(height);
                new_width = width * scale;
                new_height = max_size;
                return Resize(Convert.ToInt32(new_width), Convert.ToInt32(new_height), image);
            }
            else
            {
                throw new InvalidDataException();
            }
        }


    }
}
