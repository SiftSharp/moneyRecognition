using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime;

namespace SiftSharp.Tests
{
    [TestFixture()]
    public class ImageTests
    {
        string TestDir = TestContext.CurrentContext.TestDirectory;
        private TestContext testContextInstance;
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [Test()]
        public void ShouldFindBWFilterWhenBWFilterExist()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestDir,@"images/BW Filter.png")));
        }

        [Test()]
        public void ShouldFindInputImageWhenInputImageExist()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestDir,@"images/Input Image.png")));
        }

        [Test()]
        public void ReadImage_ShouldBeSame_WhenInputImageIsComparedToBWFilter()
        {

            System.Drawing.Bitmap BWFilter = new System.Drawing.Bitmap(Path.Combine(TestDir, @"images/BW Filter.png"));
            int width = BWFilter.Width,
                height = BWFilter.Height;
            float[,] expected = new float[width, height];
            System.Drawing.Color colors;
            LockBitmap inputLocked = new LockBitmap(BWFilter);
            inputLocked.LockBits();

            // Store grayscale value for each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors = inputLocked.GetPixel(x, y);
                    expected[x, y] = colors.R;
                }
            }

            inputLocked.UnlockBits();

            float[,] actual = Image.ReadImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir,@"images/Input Image.png")));

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void BuildImage_ShouldReturnSame_WhenInputIsBW()
        {
            System.Drawing.Bitmap BWFilter = new System.Drawing.Bitmap(Path.Combine(TestDir, @"images/BW Filter.png"));
            int width = BWFilter.Width,
                height = BWFilter.Height;
            float[,] expected = new float[width, height];
            System.Drawing.Color colors;
            LockBitmap inputLocked = new LockBitmap(BWFilter);
            inputLocked.LockBits();

            // Store grayscale value for each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors = inputLocked.GetPixel(x, y);
                    expected[x, y] = colors.R;
                }
            }

            inputLocked.UnlockBits();

            
            float[,] actual = Image.ReadImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir,@"images/BW Filter.png")));

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void GenerateGaussianKernel_ShouldReturnExactImage_WhenSigmaIsFiveDotFiveAndSizeIsThree()
        {
            System.Drawing.Bitmap GaussianFilter = new System.Drawing.Bitmap(Path.Combine(TestDir, @"images/GaussianBlurred5.5Sigma3x3Kernel.png"));
            int width = GaussianFilter.Width,
                height = GaussianFilter.Height;
            float[,] expected = new float[width, height];
            System.Drawing.Color colors;
            LockBitmap inputLocked = new LockBitmap(GaussianFilter);
            inputLocked.LockBits();

            // Store grayscale value for each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colors = inputLocked.GetPixel(x, y);
                    expected[x, y] = colors.R;
                }
            }

            inputLocked.UnlockBits();

            float[,] gaussImage = Image.Gaussian(5.5f, 3, Image.ReadImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir,@"images/Input Image.png"))));

            float[,] actual = Image.ReadImage(Image.BuildImage(gaussImage));

            Assert.AreEqual(expected, actual);
        }


    }
}