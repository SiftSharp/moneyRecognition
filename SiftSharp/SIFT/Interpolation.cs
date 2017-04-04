using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;

namespace SiftSharp.SIFT
{
    static class Interpolation
    {

        public static Feature InterpolatesExtremum(Image[][] dogPyr, int octave, int level, int x,
            int y, int numberOfLevels, float curvatureTresh)
        {
            double xLevel = 0, xRow = 0, xCol = 0;
            int maxSteps = 5;
            for (int i = 0; i < maxSteps; i++)
            {
                InterpolationStep(dogPyr, octave, level, x, y, out xLevel, out xRow, out xCol);
                if (Math.Abs(xLevel) < 0.5 && Math.Abs(xRow) < 0.5 && Math.Abs(xCol) < 0.5)
                {
                    break;
                }

                x += (int)Math.Round(xCol);
                y += (int)Math.Round(xRow);
                level += (int)Math.Round(xLevel);

                if (level < 1 || level > numberOfLevels - 2 || x < 1 || y < 1 || 
                    x >= dogPyr[octave][0].Get().GetLength(0) - 1 ||
                    y >= dogPyr[octave][0].Get().GetLength(1) - 1)
                {
                    return null;
                }
            }
            
            double contrast = InterpolatedPixelContrast(dogPyr, octave, level, x, y, xLevel, xRow, xCol);
            if (Math.Abs(contrast) < curvatureTresh / numberOfLevels)
            {
                return null;
            }

            Feature newFeature = new Feature();
            newFeature.x = (x + xCol) * Math.Pow(2.0, octave);
            newFeature.y = (y + xRow) * Math.Pow(2.0, octave);
            newFeature.level = level;
            newFeature.subLevel = xLevel;
            newFeature.octave = octave;

            return newFeature;
        }

        /// <summary>
        /// Calculates interpolated contrast. Based on Equation 3 from Lowe's paper.
        /// </summary>
        /// <param name="dogPyr" type="Image[][]">Difference-of-Gaussian pyramid</param>
        /// <param name="octave">Octaves in pyramid</param>
        /// <param name="level">Levels per octave</param>
        /// <param name="x">Pixel X coordinate</param>
        /// <param name="y">Pixel Y coordinate</param>
        /// <param name="xLevel">Interpolated subpixel increment to interval</param>
        /// <param name="xRow">Interpolated subpixel increment to row</param>
        /// <param name="xCol">Interpolated subpixel increment to col</param>
        /// <returns></returns>
        public static double InterpolatedPixelContrast(Image[][] dogPyr, int octave, int
            level, int x, int y, double xLevel, double xRow, double xCol)
        {
            Vector<double> intrVector = new DenseVector(new double[] { xCol, xRow, xLevel });
            Matrix<double> derivative = Derivative3D(dogPyr, octave, level, x, y).ToColumnMatrix();
            Vector<double> intrContrast = derivative.Transpose().Multiply(intrVector);
            return dogPyr[octave][level].Get()[x, y] + intrContrast[0] * 0.5;
        }

        /// <summary>
        /// Performs one step of extremum interpolation. Equation 3 from Lowe's paper.
        /// </summary>
        /// <param name="dogPyr" type="Image[][]">Difference-of-Gaussian pyramid</param>
        /// <param name="octave">Octaves in pyramid</param>
        /// <param name="level">Levels per octave</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="xLevel">Output param</param>
        /// <param name="xRow">Output param</param>
        /// <param name="xCol">Output param</param>
        public static void InterpolationStep(Image[][] dogPyr, int octave, int level, int x, int y,
            out double xLevel, out double xRow, out double xCol)
        {
            Vector<double> derivatives = Derivative3D(dogPyr, octave, level, x, y);
            Matrix<double> invertedHessian = Hessian3D(dogPyr, octave, level, x, y).Inverse();
            Vector<double> result = invertedHessian.Multiply(-1).Multiply(derivatives);

            xLevel = result[2];
            xRow = result[1];
            xCol = result[0];
        }

        /// <summary>
        /// Calculates Hessian Matrix from pixel in DoG scale space.
        /// </summary>
        /// <param name="dogPyramid" type="Image[][]">dogPyramid</param>
        /// <param name="x" type="int">x coordinate</param>
        /// <param name="y" type="int">y coordinate</param>
        /// <param name="octave" type="int">octave</param>
        /// <param name="level" type="int">level</param>
        /// <returns>Hessian matrix of pixel I as:
        ///    / Ixx  Ixy  Ixs \
        ///    | Ixy  Iyy  Iys |
        ///    \ Ixs  Iys  Iss /
        /// </returns>
        public static Matrix<double> Hessian3D(Image[][] dogPyramid, int octave, int level, int x, int y)
        {
            //Store images to not repeat Get call and clarity
            float[,] currentImage = dogPyramid[octave][level].Get();
            float[,] nextImage = dogPyramid[octave][level - 1].Get();
            float[,] prevImage = dogPyramid[octave][level + 1].Get();

            //pixel value at current x,y
            float value = currentImage[x, y];

            //partial derivates are being computed
            double dxx = currentImage[x + 1, y] + currentImage[x - 1, y] - 2 * value;
            double dyy = currentImage[x, y + 1] + currentImage[x, y - 1] - 2 * value;
            double dss = nextImage[x, y] + prevImage[x, y] - 2 * value;

            double dxy = (currentImage[x + 1, y + 1]) -
                        (currentImage[x - 1, y + 1]) -
                        (currentImage[x + 1, y - 1]) -
                        (currentImage[x - 1, y - 1]) / 4.0F;

            double dxs = (nextImage[x + 1, y]) -
                        (nextImage[x - 1, y]) -
                        (prevImage[x + 1, y]) +
                        (prevImage[x - 1, y]) / 4.0F;

            double dys = (nextImage[x, y + 1]) -
                        (nextImage[x, y - 1]) -
                        (prevImage[x, y + 1]) +
                        (prevImage[x, y - 1]) / 4.0F;

            // Create and return matrix from denseMatrix
            return new DenseMatrix(3, 3, new double[] {
                dxx, dxy, dxs,
                dxy, dyy, dys,
                dxs, dys, dss
            });
        }


        /// <summary>
        /// Calculates the partial derivatives in x, y, and scale of a 
        /// pixel in the DoGscale space pyramid.
        /// </summary>
        /// <param name="dogPyramid" type="Image[][]">dogPyramid</param>
        /// <param name="x" type="int">x coordinate</param>
        /// <param name="y" type="int">y coordinate</param>
        /// <param name="octave" type="int">octave</param>
        /// <param name="level" type="int">level</param>
        /// <returns>1x3 array of partial derivatives</returns>
        public static Vector<double> Derivative3D(Image[][] dogPyramid, int octave, int level, int x, int y)
        {
            //Store images to not repeat Get call and clarity
            float[,] currentImage = dogPyramid[octave][level].Get();
            float[,] nextImage = dogPyramid[octave][level - 1].Get();
            float[,] prevImage = dogPyramid[octave][level + 1].Get();

            //Partial derivatives for pixel
            double dx = (currentImage[x + 1, y] - currentImage[x - 1, y]) / 2;
            double dy = (currentImage[x, y + 1] - currentImage[x, y - 1]) / 2;
            double ds = (nextImage[x, y] - prevImage[x, y]) / 2;

            return new DenseVector(new double[] { dx, dy, ds });
        }
    }
}
