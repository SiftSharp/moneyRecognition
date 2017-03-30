﻿using NUnit.Framework;
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
        public void marioExist()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestDir,@"mario.png")));
        }

        [Test()]
        public void readImageTest()
        {
            SiftSharp.Image testImage = new SiftSharp.Image();
            int[,] expected = {
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 91, 91, 91, 139, 139, 139, 139, 139, 139 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 },
                { 139, 139, 139, 130, 130, 130, 130, 130, 130 }
            };

            int[,] result = testImage.readImage(
                new System.Drawing.Bitmap(Path.Combine(TestDir, @"mario.png")));

            Assert.AreEqual(result, expected);
        }
    }
}