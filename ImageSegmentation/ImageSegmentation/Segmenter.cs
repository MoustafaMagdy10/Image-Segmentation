using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    public class Segmenter
    {
        private readonly RGBPixel[,] img;
        private readonly int width;
        private readonly int height;
        private readonly long k;
        

        public Segmenter(RGBPixel[,] image, long kValue)
        {
            img = image;
            height = ImageOperations.GetHeight(image);
            width = ImageOperations.GetWidth(image);
            k = kValue;
        }

        private int[,] RunMonoChannel(Func<RGBPixel, byte> selector)
        {
            
            int total = width * height;
            var edges = GraphBuilder.BuildEdges(img,selector);
            edges.Sort((a, b) => a.Weight.CompareTo(b.Weight));
            var dsu = new DisjointSet(total);
            foreach (var e in edges)
            {
                int a = dsu.FindLeader(e.U);
                int b = dsu.FindLeader(e.V);
                if (a == b) continue;

                double ta = k / dsu.GetSize(a);
                double tb = k / dsu.GetSize(b);
                double ma = dsu.InternalDiff(a) + ta;
                double mb =dsu.InternalDiff(b) + tb;

                if (e.Weight <=Math.Min(ma, mb))
                {
                    dsu.Union(a, b, e.Weight);
                }

            }

             var leaders = new int[height, width];

            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    int id = y * width + x;
                    leaders[y, x] = dsu.FindLeader(id);
                }
            });
            return leaders;
        }
        int comp;
        public int[,] RunColor()
        {
            // Segmentation on R, G, B
            int[,] lr = null, lg = null, lb = null;
            var sw = Stopwatch.StartNew();
            
            Parallel.Invoke(
                () => { lr = RunMonoChannel(p => p.red); },
                () => { lg = RunMonoChannel(p => p.green); },
                () => { lb = RunMonoChannel(p => p.blue); }
            );

            sw.Stop();
            //var l = RunCombinedRGB();
            int total = width * height;
            var finalDsu = new DisjointSet(total);
            
            int[] dx = { 1, 1, -1, -1, 0, 1, 0, -1 };
            int[] dy = { 1, -1, 1, -1, 1, 0, -1, 0 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int id = y * width + x;
                    for (int i = 0; i < dx.Length; i++)
                    {
                        int nx = x + dx[i], ny = y + dy[i];
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;

                        int nid = ny * width + nx;
                        if (lr[y, x] == lr[ny, nx] &&
                            lg[y, x] == lg[ny, nx] &&
                            lb[y, x] == lb[ny, nx])
                        {
                            finalDsu.Union(id, nid, 0);
                        }

                        //if (l[y, x] == l[ny, nx])
                        //{
                        //    finalDsu.Union(id, nid, 0);
                        //}
                    }
                }
            }

            

            var finalLeaders = new int[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int id = y * width + x;
                    finalLeaders[y, x] = finalDsu.FindLeader(id);
                }
            return finalLeaders;
        }
        public RGBPixel[,] Colorize(int[,] leaders)
        {
            var rnd = new Random();
            var colors = new Dictionary<int, Color>();

            int h = leaders.GetLength(0);
            int w = leaders.GetLength(1);



            RGBPixel[,] mat = new RGBPixel[h, w];

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int leader = leaders[y, x];
                    if (!colors.ContainsKey(leader))
                        colors[leader] = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                    mat[y, x].red = colors[leader].R;
                    mat[y,x].green = colors[leader].G;
                    mat[y,x].blue = colors[leader].B;
                }
            return mat;


        }
        public (int count, List<int> sizes) GetStats(int[,] leaders)
        {
            var freq = new Dictionary<int, int>();
            int h = leaders.GetLength(0);
            int w = leaders.GetLength(1);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int leader = leaders[y, x];
                    if (!freq.ContainsKey(leader)) freq[leader] = 0;
                    freq[leader]++;
                }

            var sizes = new List<int>(freq.Values);
            sizes.Sort((a, b) => b.CompareTo(a));
            return (sizes.Count, sizes);
        }
    }
}
