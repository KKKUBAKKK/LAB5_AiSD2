using ASD.Graphs;
using System;
using System.Data.Common;
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
        public int FindShortestPath(char[,] maze, bool withDynamite, out string path, int t = 1000000)
        {
            int rows = maze.GetLength(0);
            int columns = maze.GetLength(1);
            Graph<int> graph = new Graph<int>(rows * columns);
            int start = 0;
            int end = 0;
            (int r, int c)[] moves = { (-1, 0), (0, -1), (0, 1), (1, 0) };
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int graphInd = i * columns + j;
                    // if (maze[i, j] == 'X')
                    //     continue;
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
                            else
                            {
                                graph.AddEdge(graphInd, (i + move.r) * columns + j + move.c, t);
                            }
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
        /// Wersja III i IV zadania
        /// Zwraca najkrótszy możliwy czas przejścia przez labirynt z użyciem co najwyżej k lasek dynamitu
        /// </summary>
        /// <param name="maze">labirynt</param>
        /// <param name="k">liczba dostępnych lasek dynamitu, dla wersji III k=1</param>
        /// <param name="path">zwracana ścieżka</param>
        /// <param name="t">czas zburzenia ściany</param>
        public int FindShortestPathWithKDynamites(char[,] maze, int k, out string path, int t)
        {
            path = "";
            return -1;
        }
    }
}