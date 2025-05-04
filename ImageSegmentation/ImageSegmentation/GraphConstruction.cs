using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    class GraphConstruction
    {
            public Dictionary<byte, List<byte>> Redgraph = new Dictionary<byte, List<byte>>();
            public Dictionary<byte, List<byte>> Greengraph = new Dictionary<byte, List<byte>>();
            public Dictionary<byte, List<byte>> Bluegraph = new Dictionary<byte, List<byte>>();
   

    public void AdjacencyList(RGBPixel[,] img )
    {
        int N = ImageOperations.GetHeight( img );
        int M = ImageOperations.GetWidth( img );
        
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < M; j++)
            {
                if (i == 0 && j == 0)
                {
                    Redgraph[img[i,j].red] = new List<byte> { img[i,j+1].red, img[i+1,j].red , img[i+1,j+1].red };
                    Greengraph[img[i,j].green] = new List<byte> { img[i,j+1].green, img[i+1,j].green, img[i+1,j+1].green };
                    Bluegraph[img[i,j].blue] = new List<byte> { img[i,j+1].blue, img[i+1,j].blue , img[i+1,j+1].blue };
                    
                    continue;
                }
                if(i == 0 && j ==M - 1)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i,j-1].red, img[i+1,j].red, img[i+1,j-1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i,j-1].green, img[i+1,j].green, img[i+1,j-1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i,j-1].blue, img[i+1,j].blue, img[i+1,j-1].blue };
                    continue;
                }
                if(i ==  N - 1 && j == 0)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i-1,j].red , img[i-1,j+1].red, img[i,j+1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i-1,j].green , img[i-1,j+1].green, img[i,j+1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i-1,j].blue , img[i-1,j+1].blue, img[i,j+1].blue };
                    continue;
                }
                if (i == N - 1 && j == M - 1)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i,j-1].red, img[i-1,j].red, img[i-1,j-1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i,j-1].green, img[i-1,j].green, img[i-1,j-1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i,j-1].blue, img[i-1,j].blue, img[i-1,j-1].blue };
                    continue;
                }
                if (i == 0)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i,j-1].red, img[i,j+1].red, img[i+1,j-1].red , img[i+1,j].red , img[i+1,j+1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i,j-1].green, img[i,j+1].green, img[i+1,j-1].green , img[i+1,j].green , img[i+1,j+1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i,j-1].blue, img[i,j+1].blue, img[i+1,j-1].blue , img[i+1,j].blue , img[i+1,j+1].blue };
                }
                if(j == 0)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i-1,j].red , img[i+1,j].red, img[i - 1, j + 1].red , img[i, j + 1].red, img[i + 1, j + 1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i-1,j].green , img[i+1,j].green, img[i - 1, j + 1].green , img[i, j + 1].green, img[i + 1, j + 1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i-1,j].blue , img[i+1,j].blue, img[i - 1, j + 1].blue , img[i, j + 1].blue, img[i + 1, j + 1].blue };
                }
                if(i == N - 1)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i-1,j].red , img[i+1,j].red, img[i - 1, j + 1].red , img[i, j + 1].red, img[i + 1, j + 1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i-1,j].green , img[i+1,j].green, img[i - 1, j + 1].green , img[i, j + 1].green, img[i + 1, j + 1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i-1,j].blue , img[i+1,j].blue, img[i - 1, j + 1].blue , img[i, j + 1].blue, img[i + 1, j + 1].blue };

                }
                if(j == M - 1)
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i-1,j].red, img[i+1,j].red, img[i-1,j-1].red, img[i,j-1].red, img[i+1,j-1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i-1,j].green, img[i+1,j].green, img[i-1,j-1].green, img[i,j-1].green, img[i+1,j-1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i-1,j].blue, img[i+1,j].blue, img[i-1,j-1].blue, img[i,j-1].blue, img[i+1,j-1].blue };
                }
                else
                {
                    Redgraph[img[i, j].red] = new List<byte> { img[i-1,j-1].red, img[i-1,j].red, img[i-1,j+1].red, img[i, j-1].red, img[i,j+1].red, img[i+1,j-1].red, img[i+1,j].red, img[i+1,j+1].red };
                    Greengraph[img[i, j].green] = new List<byte> { img[i-1,j-1].green, img[i-1,j].green, img[i-1,j+1].green, img[i, j-1].green, img[i,j+1].green, img[i+1,j-1].green, img[i+1,j].green, img[i+1,j+1].green };
                    Bluegraph[img[i, j].blue] = new List<byte> { img[i-1,j-1].blue, img[i-1,j].blue, img[i-1,j+1].blue, img[i, j-1].blue, img[i,j+1].blue, img[i+1,j-1].blue, img[i+1,j].blue, img[i+1,j+1].blue };
                }
            }
        }
    }
 }
    
}
