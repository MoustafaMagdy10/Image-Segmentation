using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageTemplate
{
    internal class DisjointSet
    {
        private int[] parent;
        private int[] groupSize;
        private double[] internalDiff;
        int componants;
        public DisjointSet(int n)
        {
            componants = n;
            parent = new int[n+9];
            groupSize = new int[n+9];
            internalDiff = new double[n+9];
            for (int i = 0; i <= n; i++)
            {
                parent[i] = i;
                groupSize[i] = 1;
                internalDiff[i] = 0.0;
            }
        }
        
        public int GetComponants()
        {
            return componants;
        }
        public int FindLeader(int x)
        {
            if (parent[x] == x) return x;

            return parent[x] = FindLeader(parent[x]);
        }

         public void Union(int x, int y, double weight)
         {
            int leader1 = FindLeader(x);
            int leader2 = FindLeader(y);

            if (leader1 == leader2) return;

            if (GetSize(leader1)<GetSize(leader2))
            {
                //swap
                (leader2, leader1) = (leader1, leader2);
            }

            parent[leader2] = leader1;
            groupSize[leader1] += groupSize[leader2];

            internalDiff[leader1] = Math.Max(Math.Max(internalDiff[leader1], internalDiff[leader2]), weight);

            componants--;
        }
        public int GetSize(int x)
        {
            return groupSize[FindLeader(x)];
        }
        public double InternalDiff(int x)
        {
            return internalDiff[FindLeader(x)];
        }
    }
}

