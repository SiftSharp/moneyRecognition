using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SiftSharp {

    class Program {

        const double scale = 140;

        static void Main(string[] args) {

            SiftSharp.Image r = new SiftSharp.Image("coke.jpeg");

            Bitmap out_im = new Bitmap(100, 100);
            out_im = r.scale(scale); // work, work, work, work, work, work ;-)
            out_im.Save("coke-out.png"); // Saving the resized image.
        }
    }
}
