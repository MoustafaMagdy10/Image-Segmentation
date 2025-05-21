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
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace ImageTemplate
{
    public partial class MainForm : Form
    {
        private int[,] leaders;
        private Dictionary<int, List<Point>> segmentPixels;
        private HashSet<int> selectedLabels;
        private bool isMerging = false;
        private RGBPixel[,] colorizedSegments;

        public MainForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;


            pictureBox1.Dock = DockStyle.Fill;
            pictureBox2.Dock = DockStyle.Fill;

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            segmentPixels = new Dictionary<int, List<Point>>();
            selectedLabels = new HashSet<int>();
            this.Controls.Add(Button3);
            Button3.Click += Button3_Click;


        }

        RGBPixel[,] ImageMatrix;
       
        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
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
            int maskSize = (int)nudMaskSize.Value;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);


            Stopwatch timer = Stopwatch.StartNew();


            var segmenter = new Segmenter(ImageMatrix, 35000);
            leaders = segmenter.RunColor();
            colorizedSegments = segmenter.Colorize(leaders);

        
            var (count, sizes) = segmenter.GetStats(leaders);
            timer.Stop();

            long time = timer.ElapsedMilliseconds;

            Debug.WriteLine("TIME:" + time);
            MessageBox.Show("Code Executed in " + time + " :MS");

            isMerging = false;
            Button3.Text = "Merge Segments";
            segmentPixels.Clear();
            selectedLabels.Clear();

            PopulateSegmentPixels();

            ImageOperations.DisplayImage(colorizedSegments, pictureBox2);

            
            //put the path you like , like this @"C:\Downloads"

            string outputPath = @"C:\Users\moust\source\repos\Image-Segmentation\ImageSegmentation\ImageSegmentation\MyOutput.txt";

            using (var sw = new StreamWriter(outputPath, false))
            {
                sw.WriteLine(count);

                foreach (var s in sizes)
                    sw.WriteLine(s);
            }

           

        }

        private void PopulateSegmentPixels()
        {
            segmentPixels.Clear();
            for (int y = 0; y < leaders.GetLength(0); y++)
            {
                for (int x = 0; x < leaders.GetLength(1); x++)
                {
                    int label = leaders[y, x];
                    if (!segmentPixels.ContainsKey(label))
                        segmentPixels[label] = new List<Point>();
                    segmentPixels[label].Add(new Point(x, y));
                }
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox2.MouseClick += pictureBox2_MouseClick;

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (!isMerging)
            {
                isMerging = true;
                Button3.Text = "Show Original";
                selectedLabels.Clear();
                pictureBox2.Cursor = Cursors.Hand;
            }
            else
            {
                isMerging = false;
                Button3.Text = "Merge Segments";
                pictureBox2.Cursor = Cursors.Default;

                if (selectedLabels.Count == 0)
                {
                    return;
                }
                
                 
                var result = ColorizeSelected(leaders, selectedLabels);

                ImageOperations.DisplayImage(result, pictureBox2);
                selectedLabels.Clear();
            }
        }
        public RGBPixel[,] ColorizeSelected(int[,] leaders, HashSet<int> selectedLabels)
        {
           
            int h = ImageMatrix.GetLength(0);
            int w = ImageMatrix.GetLength(1);



            RGBPixel[,] mat = new RGBPixel[h, w];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (selectedLabels.Contains(leaders[y, x]))
                    {

                        mat[y, x].red = ImageMatrix[y, x].red;
                        mat[y, x].green = ImageMatrix[y, x].green;
                        mat[y, x].blue = ImageMatrix[y, x].blue;
                    }
                    else
                    {
                        mat[y, x].red = 255;
                        mat[y, x].green = 255;
                        mat[y, x].blue = 255;

                    }
                }
            }
            return mat;


        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isMerging || ImageMatrix == null || leaders == null)
                return;

            int imgX = (int)((double)e.X / pictureBox2.Width * ImageMatrix.GetLength(1));
            int imgY = (int)((double)e.Y / pictureBox2.Height * ImageMatrix.GetLength(0));

            if (imgX < 0 || imgX >= leaders.GetLength(1) ||
                imgY < 0 || imgY >= leaders.GetLength(0)) return;

            int label = leaders[imgY, imgX];

            if (selectedLabels.Contains(label))
                selectedLabels.Remove(label);
            else
                selectedLabels.Add(label);

            HighlightSelectedRegions();
        }

        private void HighlightSelectedRegions()
        {
          
            int height = colorizedSegments.GetLength(0);
            int width = colorizedSegments.GetLength(1);
            RGBPixel[,] overlayMatrix = new RGBPixel[height, width];

            // Copy original
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    overlayMatrix[y, x] = colorizedSegments[y, x];

            // Apply white highlights to selected regions
            foreach (int label in selectedLabels)
            {
                foreach (Point p in segmentPixels[label])
                {
                    overlayMatrix[p.Y, p.X] = new RGBPixel { red = 255, green = 255, blue = 255 };
                }
            }

             ImageOperations.DisplayImage(overlayMatrix,pictureBox2);
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
            }
        }
    }
}