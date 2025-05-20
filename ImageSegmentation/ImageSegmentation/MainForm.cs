using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        private Bitmap originalImage;
        private bool selecting = true;
        private bool isMerging = false;
        private HashSet<int> mergedLabels = new HashSet<int>();
        private RGBPixel[,] originalImageMatrix;
        private RGBPixel[,] colorizedSegments;
        private Bitmap currentDisplay;

        public MainForm()
        {
            InitializeComponent();
            segmentPixels = new Dictionary<int, List<Point>>();
            selectedLabels = new HashSet<int>();
            this.Controls.Add(button3);
            button3.Click += button3_Click;


        }

        RGBPixel[,] ImageMatrix;
        private Bitmap RGBPixelToBitmap(RGBPixel[,] matrix)
        {
            int height = matrix.GetLength(0);
            int width = matrix.GetLength(1);
            Bitmap bmp = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    bmp.SetPixel(x, y, Color.FromArgb(matrix[y, x].red, matrix[y, x].green, matrix[y, x].blue));
            return bmp;
        }

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

            var st = new HashSet<int>();
            for (int i = 0; i < ImageMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < ImageMatrix.GetLength(1); j++)
                {

                    st.Add(ImageMatrix[i, j].red);


                }
            }
            foreach (var i in st)
            {
                Debug.WriteLine(i);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }



        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            //ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);


            Stopwatch timer = Stopwatch.StartNew();


            var segmenter = new Segmenter(ImageMatrix, 30000);
            leaders = segmenter.RunColor();
            originalImageMatrix = ImageMatrix;
            originalImage = RGBPixelToBitmap(originalImageMatrix);

            colorizedSegments = segmenter.Colorize(leaders);
            ImageOperations.DisplayImage(colorizedSegments, pictureBox2);



            currentDisplay = RGBPixelToBitmap(colorizedSegments);
            pictureBox2.Image = currentDisplay;
            // mergedLabels.Clear();
            isMerging = false;
            button3.Text = "Merge Segments";
            mergedLabels.Clear();
            segmentPixels.Clear();
            selectedLabels.Clear();
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
            var ImageMatrix2 = segmenter.Colorize(leaders);

            var (count, sizes) = segmenter.GetStats(leaders);
            timer.Stop();

            long time = timer.ElapsedMilliseconds;

            Debug.WriteLine("TIME:" + time);


            // ImageOperations.DisplayImage(ImageMatrix2, pictureBox2);

            /*
            //put the path you like , like this @"C:\Downloads"

            string outputPath = @"C:\Users\moust\source\repos\Image-Segmentation\ImageSegmentation\ImageSegmentation\MyOutput.txt";

            using (var sw = new StreamWriter(outputPath, false))
            {
                sw.WriteLine(count);

                foreach (var s in sizes)
                    sw.WriteLine(s);
            }

            string outputImagePath = @"C:\Users\moust\source\repos\Image-Segmentation\ImageSegmentation\ImageSegmentation\SegmentedOutput.png";
            SaveRGBPixelArrayAsImage(ImageMatrix2, outputImagePath);
        */
        }

        private void SaveRGBPixelArrayAsImage(RGBPixel[,] imageMatrix, string filePath)
        {
            int height = imageMatrix.GetLength(0);
            int width = imageMatrix.GetLength(1);
            Bitmap bmp = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    RGBPixel pixel = imageMatrix[y, x];
                    Color color = Color.FromArgb(pixel.red, pixel.green, pixel.blue);
                    bmp.SetPixel(x, y, color);
                }
            }

            bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox2.MouseClick += pictureBox2_MouseClick;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!isMerging)
            {
                isMerging = true;
                button3.Text = "Show Original";
                selectedLabels.Clear();
                pictureBox2.Cursor = Cursors.Hand;
            }
            else
            {
                isMerging = false;
                button3.Text = "Merge Segments";
                pictureBox2.Cursor = Cursors.Default;

                if (selectedLabels.Count == 0)
                {
                    MessageBox.Show("Select at least 1 segment to show");
                    return;
                }


                Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);

                for (int y = 0; y < result.Height; y++)
                {
                    for (int x = 0; x < result.Width; x++)
                    {
                        if (selectedLabels.Contains(leaders[y, x]))
                        {

                            result.SetPixel(x, y, originalImage.GetPixel(x, y));
                        }
                        else
                        {
                            result.SetPixel(x, y, Color.White);
                        }
                    }
                }

                pictureBox2.Image = result;
                selectedLabels.Clear();
            }
        }

        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isMerging || originalImage == null || leaders == null)
                return;

            int imgX = (int)((double)e.X / pictureBox2.Width * originalImage.Width);
            int imgY = (int)((double)e.Y / pictureBox2.Height * originalImage.Height);

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
            Bitmap overlay = new Bitmap(currentDisplay.Width, currentDisplay.Height);

            using (Graphics g = Graphics.FromImage(overlay))
            {

                g.DrawImage(currentDisplay, 0, 0);

                foreach (int label in selectedLabels)
                {
                    foreach (Point p in segmentPixels[label])
                    {
                        overlay.SetPixel(p.X, p.Y, Color.White);
                    }
                }
            }

            pictureBox2.Image = overlay;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedLabels.Count == 0)
            {
                MessageBox.Show("Please select at least one region first!");
                return;
            }
            Bitmap result = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    result.SetPixel(x, y, selectedLabels.Contains(leaders[y, x])
                        ? originalImage.GetPixel(x, y)
                        : Color.Black);
                }
            }
            currentDisplay = result;

            pictureBox2.Image = result;
            selectedLabels.Clear();
        }

    }
}