using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SiftSharp {
    class Program {
        const int with = 512;
        const int height = 512;
        static void Main(string[] args) {
            Size size = new Size(with, height);
            Bitmap im = new Bitmap("coke.jpeg");
            Bitmap out_im = new Bitmap(with, height);
            ResizeScale r = new ResizeScale();
            out_im = r.ResizeImage(im, size);
            out_im.Save("coke-out.png");
        }
    }
}
