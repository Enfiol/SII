using System;
using Graphviz4Net.Graphs;
using Graphviz4Net.Dot;
using Graphviz4Net.Dot.AntlrParser;
using Graphviz4Net.Primitives;
using System.IO;
using System.Linq;

namespace SII
{
    public class GraphDistance
    {
        DotGraph<string> graph;
        public void readGraph(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string input = sr.ReadToEnd();
                graph = AntlrParserAdapter<string>.GetParser().Parse(input);
            }
        }

        public int CalcDistance(string id1, string id2)
        {
            int distance = 0;
            var v1 = graph.Vertices.FirstOrDefault(v => v.Id == id1);
            var v2 = graph.Vertices.FirstOrDefault(v => v.Id == id2);

            int v1depth = 0;
            int v2depth = 0;

            var tmp = v1;
            while (tmp.Id != "Lections")
            {
                tmp = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == tmp.Id).Source.ToString());
                v1depth++;
            }

            tmp = v2;
            while (tmp.Id != "Lections")
            {
                tmp = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == tmp.Id).Source.ToString());
                v2depth++;
            }
            distance = Math.Abs(v1depth - v2depth);

            if (v1depth > v2depth)
            {
                for(int i = 0; i < v1depth-v2depth; i++)
                {
                    v1 = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == v1.Id).Source.ToString());
                }
            }
            else
            {
                for (int i = 0; i < v2depth - v1depth; i++)
                {
                    v2 = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == v2.Id).Source.ToString());
                }
            }

            while (v1.Id!=v2.Id)
            {
                v1 = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == v1.Id).Source.ToString());
                v2 = graph.Vertices.FirstOrDefault(v => v.Id == graph.Edges.FirstOrDefault(e => e.Destination.ToString() == v2.Id).Source.ToString());
                distance += 2;
            }

            return distance;
        }
    }
}
