namespace SiftSharp
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.SplitContainer = new System.Windows.Forms.SplitContainer();
            this.title = new System.Windows.Forms.Label();
            this.subtitle = new System.Windows.Forms.Label();
            this.settingsBox = new System.Windows.Forms.GroupBox();
            this.regenSet = new System.Windows.Forms.Button();
            this.checkRenderFeatures = new System.Windows.Forms.CheckBox();
            this.speechSynt = new System.Windows.Forms.CheckBox();
            this.maxImageSizeSelect = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.resultBox = new System.Windows.Forms.GroupBox();
            this.resultChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.resultLabel = new System.Windows.Forms.Label();
            this.dataProgress = new System.Windows.Forms.ProgressBar();
            this.imageTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.webcamTab = new System.Windows.Forms.TabPage();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.webcamBox = new System.Windows.Forms.PictureBox();
            this.CaptureButton = new System.Windows.Forms.Button();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).BeginInit();
            this.SplitContainer.Panel1.SuspendLayout();
            this.SplitContainer.Panel2.SuspendLayout();
            this.SplitContainer.SuspendLayout();
            this.settingsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxImageSizeSelect)).BeginInit();
            this.resultBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultChart)).BeginInit();
            this.imageTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.webcamTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webcamBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SplitContainer
            // 
            this.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            this.SplitContainer.Panel1.Controls.Add(this.title);
            this.SplitContainer.Panel1.Controls.Add(this.subtitle);
            this.SplitContainer.Panel1.Controls.Add(this.settingsBox);
            this.SplitContainer.Panel1.Controls.Add(this.resultBox);
            // 
            // SplitContainer.Panel2
            // 
            this.SplitContainer.Panel2.Controls.Add(this.dataProgress);
            this.SplitContainer.Panel2.Controls.Add(this.imageTab);
            this.SplitContainer.Size = new System.Drawing.Size(1378, 577);
            this.SplitContainer.SplitterDistance = 455;
            this.SplitContainer.SplitterWidth = 6;
            this.SplitContainer.TabIndex = 0;
            // 
            // title
            // 
            this.title.AutoSize = true;
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(8, 14);
            this.title.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(436, 55);
            this.title.TabIndex = 8;
            this.title.Text = "Money Recognition";
            // 
            // subtitle
            // 
            this.subtitle.AutoSize = true;
            this.subtitle.Location = new System.Drawing.Point(18, 80);
            this.subtitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.subtitle.Name = "subtitle";
            this.subtitle.Size = new System.Drawing.Size(357, 20);
            this.subtitle.TabIndex = 7;
            this.subtitle.Text = "Paper money recognition is used without warrenty";
            // 
            // settingsBox
            // 
            this.settingsBox.Controls.Add(this.regenSet);
            this.settingsBox.Controls.Add(this.checkRenderFeatures);
            this.settingsBox.Controls.Add(this.speechSynt);
            this.settingsBox.Controls.Add(this.maxImageSizeSelect);
            this.settingsBox.Controls.Add(this.label2);
            this.settingsBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.settingsBox.Location = new System.Drawing.Point(0, 125);
            this.settingsBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.settingsBox.Name = "settingsBox";
            this.settingsBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.settingsBox.Size = new System.Drawing.Size(455, 160);
            this.settingsBox.TabIndex = 6;
            this.settingsBox.TabStop = false;
            this.settingsBox.Text = "Settings";
            // 
            // regenSet
            // 
            this.regenSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.regenSet.Location = new System.Drawing.Point(15, 115);
            this.regenSet.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.regenSet.Name = "regenSet";
            this.regenSet.Size = new System.Drawing.Size(433, 35);
            this.regenSet.TabIndex = 6;
            this.regenSet.Text = "Regenerate Dataset";
            this.regenSet.UseVisualStyleBackColor = true;
            this.regenSet.Click += new System.EventHandler(this.regenSet_Click);
            // 
            // checkRenderFeatures
            // 
            this.checkRenderFeatures.AutoSize = true;
            this.checkRenderFeatures.Location = new System.Drawing.Point(18, 29);
            this.checkRenderFeatures.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkRenderFeatures.Name = "checkRenderFeatures";
            this.checkRenderFeatures.Size = new System.Drawing.Size(156, 24);
            this.checkRenderFeatures.TabIndex = 2;
            this.checkRenderFeatures.Text = "Render Features";
            this.checkRenderFeatures.UseVisualStyleBackColor = true;
            this.checkRenderFeatures.CheckedChanged += new System.EventHandler(this.checkRenderFeatures_CheckedChanged);
            // 
            // speechSynt
            // 
            this.speechSynt.AutoSize = true;
            this.speechSynt.Location = new System.Drawing.Point(18, 65);
            this.speechSynt.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.speechSynt.Name = "speechSynt";
            this.speechSynt.Size = new System.Drawing.Size(132, 24);
            this.speechSynt.TabIndex = 5;
            this.speechSynt.Text = "Speak results";
            this.speechSynt.UseVisualStyleBackColor = true;
            this.speechSynt.CheckedChanged += new System.EventHandler(this.speechSynt_CheckedChanged);
            // 
            // maxImageSizeSelect
            // 
            this.maxImageSizeSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.maxImageSizeSelect.Location = new System.Drawing.Point(325, 65);
            this.maxImageSizeSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.maxImageSizeSelect.Name = "maxImageSizeSelect";
            this.maxImageSizeSelect.Size = new System.Drawing.Size(123, 26);
            this.maxImageSizeSelect.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max image size";
            // 
            // resultBox
            // 
            this.resultBox.Controls.Add(this.resultChart);
            this.resultBox.Controls.Add(this.resultLabel);
            this.resultBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.resultBox.Location = new System.Drawing.Point(0, 285);
            this.resultBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resultBox.Name = "resultBox";
            this.resultBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resultBox.Size = new System.Drawing.Size(455, 292);
            this.resultBox.TabIndex = 1;
            this.resultBox.TabStop = false;
            this.resultBox.Text = "Result";
            this.resultBox.Visible = false;
            // 
            // resultChart
            // 
            this.resultChart.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.resultChart.ChartAreas.Add(chartArea1);
            this.resultChart.Dock = System.Windows.Forms.DockStyle.Bottom;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.resultChart.Legends.Add(legend1);
            this.resultChart.Location = new System.Drawing.Point(4, 85);
            this.resultChart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resultChart.Name = "resultChart";
            series1.ChartArea = "ChartArea1";
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "d";
            this.resultChart.Series.Add(series1);
            this.resultChart.Size = new System.Drawing.Size(447, 202);
            this.resultChart.TabIndex = 7;
            this.resultChart.Text = "chart1";
            // 
            // resultLabel
            // 
            this.resultLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultLabel.Location = new System.Drawing.Point(4, 24);
            this.resultLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(447, 57);
            this.resultLabel.TabIndex = 0;
            this.resultLabel.Text = "...";
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataProgress
            // 
            this.dataProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataProgress.Location = new System.Drawing.Point(0, 535);
            this.dataProgress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataProgress.Name = "dataProgress";
            this.dataProgress.Size = new System.Drawing.Size(917, 42);
            this.dataProgress.TabIndex = 0;
            // 
            // imageTab
            // 
            this.imageTab.Controls.Add(this.tabPage1);
            this.imageTab.Controls.Add(this.webcamTab);
            this.imageTab.Dock = System.Windows.Forms.DockStyle.Top;
            this.imageTab.Location = new System.Drawing.Point(0, 0);
            this.imageTab.Name = "imageTab";
            this.imageTab.SelectedIndex = 0;
            this.imageTab.Size = new System.Drawing.Size(917, 533);
            this.imageTab.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(909, 500);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Image";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(903, 494);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // webcamTab
            // 
            this.webcamTab.Controls.Add(this.pictureBox3);
            this.webcamTab.Controls.Add(this.pictureBox2);
            this.webcamTab.Controls.Add(this.webcamBox);
            this.webcamTab.Controls.Add(this.CaptureButton);
            this.webcamTab.Location = new System.Drawing.Point(4, 29);
            this.webcamTab.Name = "webcamTab";
            this.webcamTab.Padding = new System.Windows.Forms.Padding(3);
            this.webcamTab.Size = new System.Drawing.Size(909, 500);
            this.webcamTab.TabIndex = 1;
            this.webcamTab.Text = "Webcam";
            this.webcamTab.UseVisualStyleBackColor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(3, 398);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(640, 100);
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(640, 100);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // webcamBox
            // 
            this.webcamBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.webcamBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.webcamBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.webcamBox.Location = new System.Drawing.Point(3, 3);
            this.webcamBox.Margin = new System.Windows.Forms.Padding(100);
            this.webcamBox.Name = "webcamBox";
            this.webcamBox.Size = new System.Drawing.Size(640, 494);
            this.webcamBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.webcamBox.TabIndex = 1;
            this.webcamBox.TabStop = false;
            // 
            // CaptureButton
            // 
            this.CaptureButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.CaptureButton.Location = new System.Drawing.Point(716, 3);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(190, 494);
            this.CaptureButton.TabIndex = 2;
            this.CaptureButton.Text = "Capture";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // OpenDialog
            // 
            this.OpenDialog.FileName = "*.sift";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 577);
            this.Controls.Add(this.SplitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Money Recognition";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Shown += new System.EventHandler(this.MainWindow_Shown);
            this.SplitContainer.Panel1.ResumeLayout(false);
            this.SplitContainer.Panel1.PerformLayout();
            this.SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer)).EndInit();
            this.SplitContainer.ResumeLayout(false);
            this.settingsBox.ResumeLayout(false);
            this.settingsBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxImageSizeSelect)).EndInit();
            this.resultBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultChart)).EndInit();
            this.imageTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.webcamTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webcamBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer SplitContainer;
        private System.Windows.Forms.ProgressBar dataProgress;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.GroupBox resultBox;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.CheckBox checkRenderFeatures;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown maxImageSizeSelect;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox speechSynt;
        private System.Windows.Forms.GroupBox settingsBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart resultChart;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label subtitle;
        private System.Windows.Forms.Button regenSet;
        private System.Windows.Forms.TabControl imageTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage webcamTab;
        private System.Windows.Forms.PictureBox webcamBox;
        private System.Windows.Forms.Button CaptureButton;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}