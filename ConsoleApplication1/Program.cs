using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication1{
    class Program{

        private static Bitmap img;
        private static string file = "../../../cup.png";

        static void Main(string[] args){
            Console.WriteLine("Loading image...");
            try{
                img = new Bitmap(file);
            } catch (ArgumentException e){
                Console.WriteLine(
                    "{0}: {1}, probable cause is that the file wasn't found", 
                    e.GetType().Name, 
                    e.Message
                );

                return;
            }
            // Gaussian Test:
            // Single:    00:00:01.1993384
            // Parallel:  00:00:00.4254463

            // Sobel Test:
            // Single:    00:00:02.2536225
            // Parallel:  00:00:01.1190212

            // Fullsize test
            // Single:   25s
            // Parallel: 13s
            // 52% improvement with 8 cores

            //DateTime t0 = DateTime.Now;
            img = new ImageProccessing(img)
                .resize(500,500)
                .convertToBlackAndWhite()
                .Gaussian(1.5, 5)
                .SobelSupression()
                .Limit(60, 60, 60)
                .nonMaximumSurrpression()
                .build();
            //Console.WriteLine(DateTime.Now - t0);

            img.Save("cup_result.png", ImageFormat.Png);

            Bitmap merged = resultMerge(new ImageProccessing(new Bitmap(file)).resize(500,500).build(), img);

            merged.Save("beforeAndAfter.png",ImageFormat.Png);

            Process.Start(@"beforeAndAfter.png");

            Console.ReadLine();
        }
       

        static Bitmap resultMerge(Bitmap image1, Bitmap image2) {

            Bitmap bitmap = new Bitmap(
                image1.Width + image2.Width, 
                Math.Max(image1.Height, image2.Height)
            );

            using (Graphics g = Graphics.FromImage(bitmap)) {
                g.DrawImage(image1, 0, 0);
                g.DrawImage(image2, image1.Width, 0);
            }

            return bitmap;
        }
    }
}