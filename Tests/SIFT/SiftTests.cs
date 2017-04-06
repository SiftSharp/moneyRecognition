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

        


    }
}