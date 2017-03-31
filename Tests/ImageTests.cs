using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace SiftSharp.Tests
{
    [TestFixture()]
    public class ImageTests
    {
        string TestDir = TestContext.CurrentContext.TestDirectory;
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
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
        public void ShouldFindMarioWhenMarioExist()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestDir,@"mario.png")));
        }

        [Test()]
        public void ReadImage_ShouldBeSame_WhenMarioIsComparedToArray()
        {
            float[,] expected = new SiftSharp.Image(new float[,] {
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 }
            }).Get();

            float[,] actual = Image.ReadImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir, @"mario.png")));

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void BuildImage_ShouldReturnSame_WhenInputIsBW()
        {
            // Load Mario image
            System.Drawing.Bitmap marioImage = 
                new System.Drawing.Bitmap(Path.Combine(TestDir, @"mario.png"));
            
            // Get array from mario image
            float[,] result = Image.ReadImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir, @"mario.png")));

            System.Drawing.Color tempPixel;
            
            // Foreach pixel
            for(int x=0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(0); y++)
                {
                    // Get pixel at current position
                    tempPixel = marioImage.GetPixel(x, y);
                    // Assert that both are equal
                    Assert.AreEqual(
                        ((tempPixel.R + tempPixel.G + tempPixel.B) / 3), // Expected
                        result[x, y]);                                   // Actual
                }

            }
            
        }
    }
}