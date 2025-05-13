using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class GraphBuilder
    {
        public static List<Edge> BuildEdges(RGBPixel[,] img, Func<RGBPixel, byte> channelSelector)
        {
            int height = ImageOperations.GetHeight(img);
            int width = ImageOperations.GetWidth(img);
            var edges = new List<Edge>(height * width *4 );

            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int node = y * width + x;
                    byte color = channelSelector(img[y, x]);

                    for (int k = 0; k < dx.Length; k++)
                    {
                        int nx = x + dx[k];
                        int ny = y + dy[k];
                        if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                            continue;



                        int node2 = ny * width + nx;

                        if (node2 < node) continue;
                        byte color2 = channelSelector(img[ny, nx]);
                        int weight = Math.Abs(color - color2);

                    
                        edges.Add(new Edge(node, node2, weight));
                    }
                }
            }

            return edges;
        }
    }
}
