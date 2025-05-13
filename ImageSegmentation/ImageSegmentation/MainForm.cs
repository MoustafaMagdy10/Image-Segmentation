using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

            //var st =new HashSet<int>();
            //for(int i= 0; i<ImageMatrix.GetLength(0); i++)
            //{
            //    for(int j= 0; j < ImageMatrix.GetLength(1); j++)
            //    {

            //      st.Add(ImageMatrix[i, j].red);
                    

            //    }
            //}
            //foreach(var i in st)
            //{
            //    Debug.WriteLine(i);
            //}
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);


            Stopwatch timer = Stopwatch.StartNew();


            var segmenter = new Segmenter(ImageMatrix, 30000);
            int[,] leaders = segmenter.RunColor();

            var ImageMatrix2 = segmenter.Colorize(leaders);

            var (count, sizes) = segmenter.GetStats(leaders);
            timer.Stop();

            long time = timer.ElapsedMilliseconds;

            Debug.WriteLine("TIME:" + time);


            ImageOperations.DisplayImage(ImageMatrix2, pictureBox2);

            
            //put the path you like , like this @"C:\Downloads"

            string outputPath = @"C:\Users\moust\source\repos\Image-Segmentation\ImageSegmentation\ImageSegmentation\MyOutput.txt";

            using (var sw = new StreamWriter(outputPath, false))
            {
                sw.WriteLine(count);

                foreach (var s in sizes)
                    sw.WriteLine(s);
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}