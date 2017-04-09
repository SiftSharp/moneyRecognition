using SiftSharp.SIFT;
using Supercluster.KDTree;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace SiftSharp
{
    public partial class MainWindow : Form
    {
        #region Private members
        // The different kind of notes
        private static int[] nodes = new int[] { 1, 2, 5, 10, 20, 50, 100 };
        // Default max size of images
        private static int nsize = 140;
        // List of loaded features
        private List<Feature> features;
        // Test image sift instance
        private Sift siftImage;
        // Test image image instance
        private Image image;
        // Speech synthesizer for speaking results
        private SpeechSynthesizer reader = new SpeechSynthesizer();
        // Dictionary for storing votes
        private Dictionary<string, double> votes;
        // Test image result string
        private string result = "";
        // Background worker for matching dollar
        private BackgroundWorker worker;
        // KDTree of features
        private KDTree<double, Feature> tree;
        // Filename for opened file
        private string fileName;
        // Bool to indicate if image is from webcam or file
        private bool fromWebcam = false;
        // Euclidean distance for comparing features
        private static Func<double[], double[], double> L2Norm = (x, y) =>
        {
            double dist = 0;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }
            return dist;
        };
        // Variables used by webcam
        private IntPtr m_ip = IntPtr.Zero;
        private Capture cam;
        // Bitmap captured from webcam
        private Bitmap capturedImage;
        #endregion

        /// <summary>
        /// Constructor for MainForm
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Set max value for numeric selector
            maxImageSizeSelect.Maximum = 1000;
            // Set default size of images
            maxImageSizeSelect.Value = nsize;
            
            worker = new BackgroundWorker();
            // Make worker report progress
            worker.WorkerReportsProgress = true;
            // Don't accept cancelations
            worker.WorkerSupportsCancellation = false;
            // Set worker async method
            worker.DoWork += new DoWorkEventHandler(dollar_Work);
            // Set callback for progress changes
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            // Set callback for when work completed
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(dollar_WorkCompleted);

            // If the program breaks on different computers,
            // this would be a good place to start debugging
            // Setup webcam variables
            const int VIDEODEVICE = 0; // zero based index of video capture device to use
            const int VIDEOWIDTH = 640; // Depends on video device caps
            const int VIDEOHEIGHT = 480; // Depends on video device caps
            const int VIDEOBITSPERPIXEL = 24; // BitsPerPixel values determined by device

            // Start webcam
            cam = new Capture(VIDEODEVICE, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, webcamBox);
        }

        /// <summary>
        /// Event method for when the form is shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            // Load features
            LoadFolderData();

            // Create KDTree of features
            tree = new KDTree<double, Feature>(
                128, features.Select(f => f.descr).ToArray(), features.ToArray(), L2Norm);
        }

        /// <summary>
        /// Event method for when worker reports progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Set progressbar max and min to percentage
            dataProgress.Minimum = 0;
            dataProgress.Maximum = 100;

            // Set progressbar to received value
            dataProgress.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Tries to identify which bill a given image is by
        /// using KDTree with Eucledian distance and then 
        /// voting on which bill it must be. This method is
        /// also a backgroundworker delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dollar_Work(object sender, DoWorkEventArgs e)
        {
            int i = 0;

            // Received worker
            BackgroundWorker worker = sender as BackgroundWorker;

            // Load image from either file or webcam bitmap
            if (fromWebcam)
            {
                image = new Image(capturedImage);
            }else
            {
                image = new Image(fileName);
            }

            // Resize image
            image.Maxsize((int)maxImageSizeSelect.Value);

            // Find features in image
            siftImage = new Sift(image, 3);

            votes = new Dictionary<string, double>();

            // Features for each percent
            int featPercentage = (int)(siftImage.features.Length / 200.0) - 1;
            // Ensure that it isn't zero
            featPercentage = featPercentage < 1 ? 1 : featPercentage;

            // Show image loaded
            ShowImage(checkRenderFeatures.Checked);

            // Reset webcam bool to false so 
            // next image can come from file
            fromWebcam = false;

            // Parallel for loop, from 0 -> length of feature array
            Parallel.For(0, siftImage.features.Length, k =>
            {
                // Report progress if i is dividable by features per percentage
                if (i % (featPercentage) == 0)
                {
                    worker.ReportProgress((int)(i / (float)siftImage.features.Length * 100));
                }

                // Search tree for nearest neighbors
                Tuple<double[], Feature>[] test = 
                    tree.NearestNeighbors(siftImage.features[k].descr, 2);

                double distance = L2Norm(siftImage.features[k].descr, test[0].Item2.descr);

                // Ensure that Euclidean distance is below threshold
                if (distance < 0.9)
                {
                    // Invert distance to weight the vote
                    double vote = 1.0 - distance;

                    // Add vote
                    if (votes.ContainsKey(test[0].Item2.userDefined))
                    {
                        votes[test[0].Item2.userDefined] += vote;
                    }
                    else
                    {
                        votes.Add(test[0].Item2.userDefined, vote);
                    }
                }
                // Count i up to indicate feature done
                i++;
            });
            
            if(votes.Count > 0) {
                // Get the key of the winner
                string max = votes.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                // Store results
                result = max;
            }else
            {
                result = "Error";
            }
        }

        private void dollar_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(e.Error == null))
            {
                return;
            }

            dataProgress.Value = dataProgress.Maximum;
            resultLabel.Text = result;
            ShowChart();
            resultBox.Visible = true;
            Speek();
        }
        
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenDialog = new OpenFileDialog();
            OpenDialog.Filter = "Image Files|*.png;*.jpg;*.JPEG|All Files (*.*)|*.*";
            OpenDialog.FilterIndex = 1;
            OpenDialog.Multiselect = true;

            if (OpenDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = OpenDialog.FileName;
                worker.RunWorkerAsync();
            }
        }
        
        private void regenSet_Click(object sender, EventArgs e)
        {
            LoadFolderData(true);
        }

        private void checkRenderFeatures_CheckedChanged(object sender, EventArgs e)
        {
            ShowImage(checkRenderFeatures.Checked);
        }

        private void speechSynt_CheckedChanged(object sender, EventArgs e)
        {
            Speek();
        }

        public Bitmap CropImage(Bitmap source, int cropX, int cropY)
        {
            Rectangle rect = new Rectangle(cropX, cropY, source.Width - cropX, source.Height - cropY);
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(source.Width - cropX * 2, source.Height - cropY * 2);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, rect, GraphicsUnit.Pixel);
            
            return bmp;
        }
        
        private void CaptureButton_Click(object sender, EventArgs e)
        {
            capturedImage = CropImage(CaptureImage(), 0, 90);

            Debug.WriteLine(capturedImage.Height);


            fromWebcam = true;
            imageTab.SelectedTab = tabPage1;
            worker.RunWorkerAsync();
        }



        public void LoadFolderData(bool generateData = false)
        {
            if (generateData)
            {
                dataProgress.Maximum = nodes.Length;
                dataProgress.Step = 1;

                foreach (int node in nodes)
                {
                    CrawlFolder("./images/data/" + node + "/", (int)maxImageSizeSelect.Value);
                    List<Feature> feats = LoadFolder("./images/data/" + node + "/");
                    Persist.Save(feats.ToArray(), "./images/data/" + node + ".sift");
                    dataProgress.PerformStep();
                }
            }
            
            features = LoadFolder("./images/data/");
        }

        public void ShowImage(bool drawFeatures = false)
        {
            if(image == null)
            {
                return;
            }
            
            Bitmap imb = image.AsBitmap();
            if (drawFeatures)
            {
                foreach (Feature f in siftImage.features)
                {
                    imb = Draw.DrawFeature(imb, f);
                }
            }
            pictureBox1.BackgroundImage = imb;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Speaks results
        /// </summary>
        private void Speek()
        {
            if (speechSynt.Checked)
            {
                reader.Speak(result);
            }
        }

        private void ShowChart()
        {
            resultChart.Series.Clear();
            List<int> sortedKeys = votes.Keys.Select(e => int.Parse(e)).ToList();
            sortedKeys.Sort();
            Series series = resultChart.Series.Add("Results");
            foreach(int k in sortedKeys)
            {
                string key = string.Format("{0}", k);
                series.Points.AddXY(key, votes[key]);
            }
        }

        public List<Feature> LoadFolder(string path, int limit = 0)
        {
            string[] files = Directory.GetFiles(path, "*.sift");

            if(files.Length == 0)
            {
                LoadFolderData(true);
            }

            int fileMax;
            if (limit != 0)
            {
                fileMax = (files.Length > limit ? limit : files.Length);
            }
            else
            {
                fileMax = files.Length;
            }

            List<Feature> features = new List<Feature>();

            int m = (int)Math.Ceiling(fileMax / 1000.0) + 1;

            for (int i = 0; i < fileMax; i++)
            {
                features.AddRange(Persist.Load(files[i]));
            }

            return features;
        }

        public static void CrawlFolder(string path, int maxSize = 0)
        {
            List<string> fileList = Directory.GetFiles(path, "*.png").ToList();
            fileList.AddRange(Directory.GetFiles(path, "*.jpg"));

            string[] files = fileList.ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                string f = files[i];
                Image image = new Image(f);

                if (maxSize > 0 && image.Get().GetLength(0) > maxSize || image.Get().GetLength(1) > maxSize)
                {
                    image.Maxsize(maxSize);
                }

                Sift sift = new Sift(image, 3);
                Persist.Save(
                    sift.features,
                    Path.Combine(path, "features") + Path.GetFileNameWithoutExtension(f) + ".sift"
                );
            }
        }

        #region Webcam
        private Bitmap CaptureImage()
        {
            // Release any previous buffer
            if (m_ip != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(m_ip);
                m_ip = IntPtr.Zero;
            }

            // capture image
            m_ip = cam.Click();
            Bitmap b = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, m_ip);

            // If the image is upsidedown
            b.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return b;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            cam.Dispose();

            if (m_ip != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(m_ip);
                m_ip = IntPtr.Zero;
            }
        }
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);

            if (m_ip != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(m_ip);
                m_ip = IntPtr.Zero;
            }
        }
        #endregion

    }
}
