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

            var st =new HashSet<int>();
            for(int i= 0; i<ImageMatrix.GetLength(0); i++)
            {
                for(int j= 0; j < ImageMatrix.GetLength(1); j++)
                {

                  st.Add(ImageMatrix[i, j].red);
                    

                }
            }
            foreach(var i in st)
            {
                Debug.WriteLine(i);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private RGBPixel[,] BitmapToRGBPixel(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            var mat = new RGBPixel[h, w];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    mat[y, x].red = c.R;
                    mat[y, x].green = c.G;
                    mat[y, x].blue = c.B;
                }
            return mat;
        }
        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            //ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);



            /* <---debuging---> */
            //var st = new HashSet<int>();
            //for (int i = 0; i < ImageMatrix.GetLength(0); i++)
            //{
            //    for (int j = 0; j < ImageMatrix.GetLength(1); j++)
            //    {

            //        st.Add(ImageMatrix[i, j].red);


            //    }
            //}
            //foreach (var i in st)
            //{
            //    Debug.WriteLine(i);
            //}


            var segmenter = new Segmenter(ImageMatrix, 300);
            int[,] leaders = segmenter.RunColor();

            var ImageMatrix2 = segmenter.Colorize(leaders);


            ImageOperations.DisplayImage(ImageMatrix2, pictureBox2);

            var (count, sizes) = segmentter.GetStats(leaders);

            //put the path you like , like this @"C:\.."
            string outputPath = Path.Combine(Application.StartupPath, "output.txt");

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