using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp {
    class Program {
<<<<<<< HEAD
		static void Main(string[] args)
		{

		}
=======
        static void Main(string[] args) {
            var i = new Image("../../../images/mario.png");

            i.Gaussian(5.5f,5);
        }
>>>>>>> 65ea957d6f37721cca5d3452ff23466644fbe2d8
    }
}
