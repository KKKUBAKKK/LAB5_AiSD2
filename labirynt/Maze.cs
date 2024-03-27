using ASD.Graphs;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;

namespace ASD
{
    public class Maze : MarshalByRefObject
    {

        /// <summary>
        /// Wersje zadania I oraz II
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt bez dynamitów lub z dowolną ich liczbą
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="withDynamite">informacja, czy dostępne są dynamity 
        /// Wersja I zadania -> withDynamites = false, Wersja II zadania -> withDynamites = true</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany (dotyczy tylko wersji II)</param> 
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 0)
        {
            int rows = maze.GetLength(0);
            int columns = maze.GetLength(1);
            DiGraph<int> graph = new DiGraph<int>(rows * columns);
            int start = 0;
            int end = 0;
            (int r, int c)[] moves = { (-1, 0), (0, -1), (0, 1), (1, 0) };
            
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    int graphInd = i * columns + j;
                    if (!withDynamite && maze[i, j] == 'X')
                        continue;
                    if (maze[i, j] == 'S')
                        start = graphInd;
                    if (maze[i, j] == 'E')
                        end = graphInd;
                    foreach (var move in moves)
                    {
                        if (i + move.r < rows && i + move.r >= 0 && j + move.c < columns && j + move.c >= 0)
                        {
                            if (maze[i + move.r, j + move.c] != 'X')
                            {
                                graph.AddEdge(graphInd, (i + move.r) * columns + j + move.c, 1);
                            }
                            else if (withDynamite)
                            {
                                graph.AddEdge(graphInd, (i + move.r) * columns + j + move.c, t);
                            }
                        }
                    }
                }

            var pathInfo = Paths.Dijkstra(graph, start);
            if (!pathInfo.Reachable(start, end))
            {
                path = "";
                return -1;
            }

            var graphPath = pathInfo.GetPath(start, end);
            StringBuilder stringBuilder = new StringBuilder();
            int prev = start;
            foreach (var vertInd in graphPath)
            {
                if (vertInd == prev - columns)
                {
                    prev = prev - columns;
                    stringBuilder.Append("N");
                    continue;
                }
                if (vertInd == prev + columns)
                {
                    prev = prev + columns;
                    stringBuilder.Append("S");
                    continue;
                }
                if (vertInd == prev - 1)
                {
                    prev = prev - 1;
                    stringBuilder.Append("W");
                    continue;
                }
                if (vertInd == prev + 1)
                {
                    prev = prev + 1;
                    stringBuilder.Append("E");
                }
            }

            path = stringBuilder.ToString();
            return pathInfo.GetDistance(start, end);
        }

        /// <summary>
        /// Wersja III i IV zadania - O(nk log(nk))
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            int rows = maze.GetLength(0);
            int columns = maze.GetLength(1);
            int n = rows * columns;
            int dynamite_level = 0;
            int start = 0;
            int end = 1;
            DiGraph<int> graph = new DiGraph<int>((k + 1) * n);
            int[] dynamites_used = new int[k + 1];
            (int r, int c)[] moves = { (-1, 0), (0, -1), (0, 1), (1, 0) };
            
            for (int level = 0; level < k + 1; level++)
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        int ind = i * columns + j;
                        
                        if (maze[i, j] == 'S')
                            start = ind;
                        else if (maze[i, j] == 'E')
                            end = ind;
                        
                        foreach (var move in moves)
                        {
                            int ti = i + move.r;
                            int tj = j + move.c;
                            int tind = ti * columns + tj;
                            if (0 <= ti && ti < rows && 0 <= tj && tj < columns)
                            {
                                if (maze[ti, tj] == 'X')
                                {
                                    if (level >= k)
                                        continue;
                                    graph.AddEdge(ind + level * n, tind + (level + 1) * n, t);
                                }
                                else
                                {
                                    graph.AddEdge(ind + level * n, tind + level * n, 1);
                                }
                            }
                        }
                    }

            var pathInfo = Paths.Dijkstra(graph, start);
            int min_cost = Int32.MaxValue;
            int min_end = 0;
            for (int level = 0; level < k + 1; level++)
            {
                int curr_end = end + level * n;
                int curr_cost = (pathInfo.Reachable(start, curr_end))
                    ? pathInfo.GetDistance(start, curr_end) : Int32.MaxValue;

                if (min_cost > curr_cost)
                {
                    min_end = curr_end;
                    min_cost = curr_cost;
                }
            }
            
            if (min_cost == Int32.MaxValue)
            {
                path = "";
                return -1;
            }
            
            path = "";
            return min_cost;        
        }
    }
}