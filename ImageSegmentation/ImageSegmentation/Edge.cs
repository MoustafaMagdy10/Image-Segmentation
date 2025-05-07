using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class Edge
    {
        public int U, V;
        public int Weight;
        public Edge(int u, int v, int w) { U = u; V = v; Weight = w; }
    }
}
