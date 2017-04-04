using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp.SIFT
{
    public static class Draw
    {
        /// <summary>
        /// Overload method for DrawFeature
        /// </summary>
        /// <param name="bitmap">Input bitmap</param>
        /// <param name="feat" type="Feature">Feature</param>
        /// <returns></returns>
        public static Bitmap DrawFeature(Bitmap bitmap, Feature feat)
        {
            return DrawFeature(bitmap, (int)feat.x, (int)feat.y, (int)Math.Round(Sift.orientationRadius * feat.scale),
                (float)feat.orientation, feat.level);
        }

        /// <summary>
        /// Draws a circle with a line indicating both scale and orientation of keypoint
        /// </summary>
        /// <param name="bitmap">Input bitmap</param>
        /// <param name="x">X coordinate of keypoint</param>
        /// <param name="y">Y coordinate of keypoint</param>
        /// <param name="radius">Radius of keypoint</param>
        /// <param name="orientation">Percentage of full circle</param>
        /// <returns>Returns same bitmap as input but with drawn circle and line</returns>
        public static Bitmap DrawFeature(Bitmap bitmap, int x, int y, int radius, float orientation, int level)
        {
            // Array of hex codes for bright neon colors
            string[] neonColors = new string[] {
                "#FFFF00","#FFFF33","#F2EA02","#E6FB04","#FF0000","#FD1C03",
                "#FF3300","#FF6600","#00FF00","#00FF33","#00FF66","#33FF00",
                "#00FFFF","#099FFF","#0062FF","#0033FF","#FF00FF","#FF00CC",
                "#FF0099","#CC00FF","#9D00FF","#CC00FF","#6E0DD0","#9900FF"
            };

            // Create graphics instance from bitmap
            Graphics g = Graphics.FromImage(bitmap);

            // Create instance of pen with random color
            Pen p = new Pen(ColorTranslator.FromHtml(neonColors[level]), 2F);

            // Draw circle with given radius
            g.DrawEllipse(p, x - radius, y - radius, radius * 2, radius * 2);

            // Calculate radians from float
            double radians = -(orientation * (2 * Math.PI));

            // Determine second point in orientation line
            int cx = x + (int)Math.Round(radius * Math.Cos(radians));
            int cy = y + (int)Math.Round(radius * Math.Sin(radians));

            // Draw line illustrating orientation
            g.DrawLine(p, new Point(x, y), new Point(cx, cy));

            return bitmap;
        }
    }
}
