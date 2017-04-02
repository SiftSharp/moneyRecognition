using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp {
    class Program {
        static void Main(string[] args)
        {
           Image mario = new Image(@"../../../images/morten.jpg");
           Bitmap result = mario.buildImage(mario.Gaussian(5.5f));

            result.Save(@"../../../images/result.png");

        }
    }
}
