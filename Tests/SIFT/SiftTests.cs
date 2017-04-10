using NUnit.Framework;
using SiftSharp.SIFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp.SIFT.Tests
{
    [TestFixture()]
    public class SiftTests
    {
        [Test()]
        public void IsExtremum_ShouldNotBeExtremum()
        {
            float[,] emptyArray = new float[,]
            {
                { 0.0F, 1.0F, 0.0F},
                { 1.0F, 1.0F, 1.0F},
                { 0.0F, 1.0F, 0.0F},
            };

            float[,] middleArray = new float[,]
            {
                { 0.0F, 1.0F, 0.0F },
                { 1.0F, 0.0F, 1.0F },
                { 0.0F, 1.0F, 0.0F },
            };

            Image[][] fakeDogPyramid = new Image[][]
            {
                new Image[]{
                    new Image(emptyArray),
                    new Image(middleArray),
                    new Image(emptyArray)
                }
            };

            Assert.IsFalse(Sift.IsExtremum(fakeDogPyramid, 1, 1, 0, 1));
        }


        [Test()]
        public void IsExtremum_ShouldBeExtremum()
        {
            float[,] emptyArray = new float[,]
            {
                { 0.0F, 1.0F, 0.0F},
                { 1.0F, 1.0F, 1.0F},
                { 0.0F, 1.0F, 0.0F},
            };

            float[,] middleArray = new float[,]
            {
                { 0.0F, 1.0F, 0.0F },
                { 1.0F, 10.0F, 1.0F },
                { 0.0F, 1.0F, 0.0F },
            };

            Image[][] fakeDogPyramid = new Image[][]
            {
                new Image[]{
                    new Image(emptyArray),
                    new Image(middleArray),
                    new Image(emptyArray)
                }
            };

            Assert.IsTrue(Sift.IsExtremum(fakeDogPyramid, 1, 1, 0, 1));
        }

        [Test()]
        public void Histogram_Should()
        {
            int x = 2, y = 2, bins = 4, radius = 2;
            double sigma = 1.5F;

            float[,] keypoint = new float[,]
            {
                { 1.0F, 2.0F, 3.0F, 4.0F, 5.0F },
                { 1.0F, 2.0F, 3.0F, 4.0F, 5.0F },
                { 1.0F, 2.0F, 3.0F, 4.0F, 5.0F },
                { 1.0F, 2.0F, 3.0F, 4.0F, 5.0F },
                { 1.0F, 2.0F, 3.0F, 4.0F, 5.0F },
            };

            Image keypointImage = new Image(keypoint);

            double[] histogram = Sift.Histogram(
                keypointImage, x, y, bins, radius, sigma);    


        }



    }
}