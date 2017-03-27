using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp
{
    class ResizeScale
    {
        /*
        Function ResizeImage takes a bitmap and a size as input and gives a new resized 
        bitmap as output. We are using the buillt in classes System.Drawing to do the job. 
        */
        public Bitmap ResizeImage (Bitmap img, Size size)
        { 
            try
            {
                Bitmap b = new Bitmap(size.Width, size.Height); // Create a new bitmap.
                using (Graphics g = Graphics.FromImage(b))
                {
                    // sets interpolationmode to bicubic.
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    // interpolate the image.
                    g.DrawImage(img, 0, 0, size.Width, size.Height);
                }
                return b; // returning the resized bitmap.
            }
            catch  // Errorhandeling here.
            {
                Console.WriteLine("Bitmap could not be resized"); // Error msg.
                return img; // Return the non resized bitmap.
            }
        }
    }
}
