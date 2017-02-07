using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication1{
    class Program{

        private static Bitmap img;
        private static string file = "bananas.png";

        static void Main(string[] args){
            Console.WriteLine("Loading image...");
            try{
                img = new Bitmap(file);
            } catch (ArgumentException e){
                Console.WriteLine("{0}: {1}, probable cause is that the file wasn't found", e.GetType().Name, e.Message);
            }

            img = new ImageProccessing(img)
                .Gaussian(1.5, 3)
                .build();

            img.Save("blurred.png", ImageFormat.Png);

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}