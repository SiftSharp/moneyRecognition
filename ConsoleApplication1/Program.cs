using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleApplication1{
    class Program{

        private static Bitmap img;
        private static string[] files = {
            "text.png",
            "text_logo.png",
            "checkerboard.png",
            "cup.png",
            "cup2.png",
            "object.png",
            "wikiExample.png",
            "wikiExample2.png",
            "pepsi.png",
            "Corner.png"
        };
        private static int chosenFile = 9;
        private static Canny cannyData;

        static void Main(string[] args) {
            int width, height;
            Console.WriteLine("Loading image...");
            try {
                img = new Bitmap("../../../objects/" + files[chosenFile]);
            } catch (ArgumentException e) {
                Console.WriteLine(
                    "{0}: {1}, probable cause is that the file wasn't found",
                    e.GetType().Name,
                    e.Message
                );

                return;
            }
            width = height = 500;

            DateTime t0 = DateTime.Now;

            cannyData = new Canny(img, width, height);
            img = cannyData.buildImage(cannyData.hcr2);
            Console.WriteLine(DateTime.Now - t0);

            img.Save("results.png", ImageFormat.Png);

            Bitmap merged = resultMerge(new ImageProccessing(new Bitmap("../../../objects/" + files[chosenFile])).resize(width, height).build(), img);

            merged.Save("beforeAndAfter.png", ImageFormat.Png);

            Process.Start(@"beforeAndAfter.png");

            //Console.Read();
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