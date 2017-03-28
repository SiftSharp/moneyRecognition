using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiftSharp {
    internal class Program {
		private static void Main(string[] args)
		{
			const string path = "../../../images/img1.jpg";
			var image = new Image(path);

		    //Testcase for SlidingWindow()
		    var testfilters = new double[2][,];
		    double[,] testfilter = {
		        {1, 1, 1},
		        {1, 1, 1},
		        {1, 1, 1}
		    };
		    double[,] testfilter2 = {
		        {2, 1, 2},
		        {1, 1, 1},
		        {2, 1, 2}
		    };
		    testfilters[0] = testfilter;
		    testfilters[1] = testfilter2;
		    double[,] convTestImage = {
		        {17, 14, 13, 09, 17 },
		        {21, 64, 62, 41, 19 },
		        {42, 54, 61, 52, 40},
		        {41, 30, 31, 34, 38},
		        {20, 24, 40, 38, 35},
		    };
		    var ax = image.SlidingWindow(convTestImage, testfilters, Image.SlideTypes.Convolution);

		    for (var i = 0; i < 2; i++)
		    {
		        Console.Write
		        ("Result: " +
		         "\n{0} {1} {2} {3} {4}\n" +
		         "\n{5} {6} {7} {8} {9}\n" +
		         "\n{10} {11} {12} {13} {14}\n" +
		         "\n{15} {16} {17} {18} {19}\n" +
		         "\n{20} {21} {22} {23} {24}\n",
		            ax[i][0, 0], ax[i][1, 0], ax[i][2, 0], ax[i][3, 0], ax[i][4, 0],
		            ax[i][0, 1], ax[i][1, 1], ax[i][2, 1], ax[i][3, 1], ax[i][4, 1],
		            ax[i][0, 2], ax[i][1, 2], ax[i][2, 2], ax[i][3, 2], ax[i][4, 2],
		            ax[i][0, 3], ax[i][1, 3], ax[i][2, 3], ax[i][3, 3], ax[i][4, 3],
		            ax[i][0, 4], ax[i][1, 4], ax[i][2, 4], ax[i][3, 4], ax[i][4, 4]);
		    }
		}
    }
}
