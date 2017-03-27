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
            Bitmap out_im = new Bitmap(with, height);
            SiftSharp.Image r = new SiftSharp.Image("coke.jpeg");
            out_im = r.resize(size);
            out_im.Save("coke-out.png");
        }
    }
}
