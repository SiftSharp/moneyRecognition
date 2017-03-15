using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ConsoleApplication1 {
    public partial class Main : Form {
        private static Bitmap img;
        private static Bitmap output;
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
        private static int chosenFile = 3;
        private static Canny cannyData;
        private int width;
        private int height;
        private DateTime t0;
        private float sigma;
        private float maxHysteresisThresh;
        private float minHysteresisThresh;

        public Main() {
            this.sigma = 1.4F;
            width = height = 500;
            maxHysteresisThresh = 35F;
            minHysteresisThresh = 25F;
            openImage("../../../objects/" + files[chosenFile]);
            InitializeComponent();
            
        }

        private void Main_Load(object sender, EventArgs e) {
            
            pictureBox.Image = cannyData.buildImage(cannyData.edgeMap);

            numericUpDown2.Value = (decimal) maxHysteresisThresh;
            numericUpDown1.Value = (decimal) minHysteresisThresh;

            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Items.Add("Normal Image");
            comboBox1.Items.Add("Grayscale Image");
            comboBox1.Items.Add("Filtered Image");
            comboBox1.Items.Add("Gradient Image");
            comboBox1.Items.Add("Non-maximum suppression");
            comboBox1.Items.Add("Derivative X");
            comboBox1.Items.Add("Derivative Y");
            comboBox1.Items.Add("Derivative XY");
            comboBox1.Items.Add("GNL");
            comboBox1.Items.Add("GNH");
            comboBox1.Items.Add("Edge Map Image");
            comboBox1.Items.Add("Harris Corners");
            comboBox1.Items.Add("Harris Corners and Edges");
        }
        
        private void openImage(string path) {
            try {
                img = new Bitmap(path);
            }
            catch (ArgumentException ex) {
                Console.WriteLine(
                    "{0}: {1}, probable cause is that the file wasn't found",
                    ex.GetType().Name,
                    ex.Message
                );
                return;
            }
            if(gaussianTrackbar != null) {
                updateImage();
            }else {
                cannyData = new Canny(img, width, height, sigma, maxHysteresisThresh, minHysteresisThresh);
            }

            if(comboBox1 != null) {
                comboBox1_SelectedIndexChanged(null, null);
            }
        }

        private void pictureBox_Click(object sender, EventArgs e) {

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                openImage(openFileDialog1.FileName);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //Console.Out.WriteLine("Selected Index: "+comboBox1.SelectedIndex);
            switch (comboBox1.SelectedIndex) {
                case 0:
                    pictureBox.Image = (Bitmap) img.Clone();
                    break;
                case 1:
                    pictureBox.Image = cannyData.buildImage(cannyData.greyImage);
                    break;
                case 2:
                    pictureBox.Image = cannyData.buildImage(cannyData.filteredImage);
                    break;
                case 3:
                    pictureBox.Image = cannyData.buildImage(cannyData.gradient);
                    break;
                case 4:
                    pictureBox.Image = cannyData.buildImage(cannyData.nonMax);
                    break;
                case 5:
                    pictureBox.Image = cannyData.buildImage(cannyData.derivativeX);
                    break;
                case 6:
                    pictureBox.Image = cannyData.buildImage(cannyData.derivativeY);
                    break;
                case 7:
                    pictureBox.Image = cannyData.buildImage(cannyData.derivativeXY);
                    break;
                case 8:
                    pictureBox.Image = cannyData.buildImage(cannyData.GNL);
                    break;
                case 9:
                    pictureBox.Image = cannyData.buildImage(cannyData.GNH);
                    break;
                case 10:
                    pictureBox.Image = cannyData.buildImage(cannyData.edgeMap);
                    break;
                case 11:
                    pictureBox.Image = cannyData.buildImage(cannyData.hcr);
                    break;
                case 12:
                    pictureBox.Image = cannyData.buildImage(cannyData.hcr2);
                    break;
                default:
                    break;
            }
        }

        private void gaussianTrackbar_Scroll(object sender, EventArgs e) {
            Console.Out.WriteLine("Gaussian Sigma: " + gaussianTrackbar.Value);
            gaussianTrackbar.Enabled = false;
            updateImage();
            gaussianTrackbar.Enabled = true;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
            maxHysteresisThresh = (float)numericUpDown2.Value;
            updateImage();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            minHysteresisThresh = (float)numericUpDown1.Value;
            updateImage();
        }
        private void updateImage() {
            cannyData = new Canny(img, width, height, (float)Math.Pow(gaussianTrackbar.Value + 1, sigma), maxHysteresisThresh, minHysteresisThresh);
            comboBox1_SelectedIndexChanged(null, null);
        }

        private void button1_Click(object sender, EventArgs e) {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                pictureBox.Image.Save(saveFileDialog1.FileName, ImageFormat.Png);
            }
        }
    }
}
