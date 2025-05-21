using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageTemplate
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Normal;


            pictureBox1.Dock = DockStyle.Fill;
            pictureBox2.Dock = DockStyle.Fill;

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

          




        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                Console.WriteLine(ImageMatrix.ToString());
            }

            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);


            Stopwatch timer = Stopwatch.StartNew();


            var segmenter = new Segmenter(ImageMatrix, 30000); //O(1)
            int[,] leaders = segmenter.RunColor(); // N Log N

            var ImageMatrix2 = segmenter.Colorize(leaders); // O(N)

            var (count, sizes) = segmenter.GetStats(leaders); // O(n + s log s)
            timer.Stop();

            long time = timer.ElapsedMilliseconds;

            Debug.WriteLine("TIME:" + time);


            ImageOperations.DisplayImage(ImageMatrix2, pictureBox2);


            

            string outputPath = @"D:\Algorithims project\Image-Segmentation\ImageSegmentation\ImageSegmentation\MyOutput.txt";

            using (var sw = new StreamWriter(outputPath, false)) // O(leaders) ;
            {
                sw.WriteLine(count);

                foreach (var s in sizes)
                    sw.WriteLine(s);
            }


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
            }


        }
     

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}