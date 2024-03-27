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
            int[] prev = new int[G.VertexCount];
            int endTime = 0;
            int width = Int32.MinValue;
            
            List<int> path = new List<int>();

            // for (int k = maxWeight; k > 0; k--)
            for (int k = 1; k <= maxWeight; k++)
            {
                SafePriorityQueue<int, int> queue = new SafePriorityQueue<int, int>();
                
                int[] currTime = new int[G.VertexCount];
                for (int i = 0; i < currTime.Length; i++)
                    currTime[i] = Int32.MinValue;
                
                int[] currPrev = new int[G.VertexCount];
                int[] currWidth = new int[G.VertexCount];

                currTime[start] = 0;
                currWidth[start] = Int32.MaxValue;
                queue.Insert(start, 0);
                while (queue.Count > 0)
                {
                    var v = queue.Extract();
                    if (v == end || currTime[v] == Int32.MinValue)
                        break;

                    foreach (var n in G.OutNeighbors(v))
                    {
                        if (G.GetEdgeWeight(v, n) >= k)
                        {
                            if (currTime[n] == Int32.MinValue)
                            {
                                currTime[n] = currTime[v] + weights[n];
                                currPrev[n] = v;
                                currWidth[n] = (G.GetEdgeWeight(v, n) > currWidth[v])
                                    ? currWidth[v] : G.GetEdgeWeight(v, n);
                                queue.Insert(n, currTime[n]);
                            }
                            else if (currTime[n] > currTime[v] + weights[n])
                            {
                                currTime[n] = currTime[v] + weights[n];
                                currPrev[n] = v;
                                currWidth[n] = (G.GetEdgeWeight(v, n) > currWidth[v])
                                    ? currWidth[v] : G.GetEdgeWeight(v, n);
                                queue.UpdatePriority(n, currTime[n]);
                            }
                        }
                    }
                }

                if (width - endTime < currWidth[end] - currTime[end] && currTime[end] != Int32.MinValue)
                // if (width - endTime < k - currTime[end] && currTime[end] != Int32.MinValue)
                {
                    width = currWidth[end];
                    endTime = currTime[end];
                    prev = currPrev;
                    if (k != width)
                        k = width - 1;
                    // break;
                }
            }

            if (endTime == Int32.MinValue)
                return path;
            
            int p = end;
            while (p != start || p != 0)
            {
                path.Add(p);
                p = prev[p];
            }
            path.Add(start);
            path.Reverse();
            
            return path;
        }
    }
}