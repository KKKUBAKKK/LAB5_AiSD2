using System.Collections;
using System.Linq;

namespace ASD
{
    using ASD.Graphs;
    using System;
    using System.Collections.Generic;

    public class Lab06 : System.MarshalByRefObject
    {
        public List<int> WidePath(DiGraph<int> G, int start, int end)
        {
            SafePriorityQueue<int, int> queue = new SafePriorityQueue<int, int>(Comparer<int>.Create((x, y) => y.CompareTo(x)), G.VertexCount);
            
            int[] prev = new int[G.VertexCount];
            int[] width = new int[G.VertexCount];
            for (int i = 0; i < width.Length; i++)
                width[i] = Int32.MinValue;
            
            List<int> path = new List<int>();
            
            queue.Insert(start, 0);
            width[start] = Int32.MaxValue;
            while (queue.Count > 0)
            {
                var v = queue.Extract();
                if (v == end || width[v] == Int32.MinValue)
                {
                    break;
                }

                foreach (var n in G.OutNeighbors(v))
                {
                    int w = G.GetEdgeWeight(v, n);
                    if (width[n] == Int32.MinValue)
                    {
                        width[n] = (w < width[v]) ? w : width[v];
                        prev[n] = v;
                        queue.Insert(n, width[n]);
                    }
                    else if (w > width[n] && width[v] > width[n])
                    {
                        width[n] = (w < width[v]) ? w : width[v];
                        prev[n] = v;
                        queue.UpdatePriority(n, width[n]);
                    }
                }
            }

            if (width[end] == Int32.MinValue)
                return path;
            
            int p = end;
            while (p != start)
            {
                path.Add(p);
                p = prev[p];
            }
            path.Add(start);
            path.Reverse();
            
            return path;
        }


        public List<int> WeightedWidePath(DiGraph<int> G, int start, int end, int[] weights, int maxWeight)
        {
            return new List<int>();
        }
    }
}