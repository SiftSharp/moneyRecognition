using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp {
    class Program {
        static void Main(string[] args) {
            var i = new Image("../../../images/mario.png");

            i.Gaussian(5.5f,5);
        }
    }
}
