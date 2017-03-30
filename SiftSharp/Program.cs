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
            var i = new Image("../../../images/mario.png");

            var gauss = i.Gaussian(5.5f);
            Console.WriteLine("Gauss image:");
            for (int x = 0; x < gauss.GetLength(0); x++)
            {

                Console.Write("[ ");
                for (int y = 0; y < gauss.GetLength(1); y++)
                {
                    Console.Write($"{gauss[x,y]}, ");
                }
                Console.Write(" ]");
                Console.WriteLine();
            }
        }
    }
}
