using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace OS_Lab4
{
    class Vertex
    {
        public string Name { get; set; }
        public int Weight { get; set; }
        public int Id { get; set; }

        public Vertex(string name, int weight, int id)
        {
            Name = name;
            Weight = weight;
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            return this.Equals((Vertex)obj);
        }

        protected bool Equals(Vertex other)
        {
            return Name == other.Name && Weight == other.Weight && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Weight, Id);
        }
    }
    class Edge
    {
        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public int Weight { get; set; }

        public Edge(Vertex start, Vertex end, int weight)
        {
            Start = start;
            End = end;
            Weight = weight;
        }

        public override bool Equals(object? obj)
        {
            return this.Equals((Edge) obj);
        }

        protected bool Equals(Edge other)
        {
            return Equals(Start, other.Start) && Equals(End, other.End) && Weight == other.Weight;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, Weight);
        }
    }
    class Graph
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public Dictionary<Vertex, List<Edge>> Adjustments { get; set; }
        public SortedDictionary<int, List<Vertex>> Map { get; set; }
        public Graph()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
            Adjustments = new Dictionary<Vertex, List<Edge>>();
            Map = new SortedDictionary<int, List<Vertex>>();
        }

        /*private List<Vertex> GetLevel(Vertex start)
        {
            var result = new List<Vertex>();
            foreach (var edge in Adjustments[start])
            {
                result.Add(edge.End);
            }
            return result;
        }*/

        public void AddNode(Vertex vertex) => Vertices.Add(vertex);

        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);

            if (!Adjustments.ContainsKey(edge.Start))
                Adjustments.Add(edge.Start, new List<Edge>());

            if (!Adjustments.ContainsKey(edge.End))
                Adjustments.Add(edge.End, new List<Edge>());

            Adjustments[edge.Start].Add(edge);
        }

        public void AddEdge(Vertex start, Vertex end, int weight) => AddEdge(new Edge(start, end, weight));

        private Vertex FindByName(string name) => Vertices.FirstOrDefault(v => v.Name == name);

        public List<Vertex> GetCriticalPath(string startName, string endName)
        {
            var start = FindByName(startName);
            var end = FindByName(endName);
            var verts = new List<Vertex>();
            CalculatePath(start,end, 0, verts);
            return Map.Last().Value;
        }

        public void CalculatePath(Vertex start, Vertex end, int path, List<Vertex> visited)
        {
            visited.Add(start);
            if (start.Equals(end))
            {
                path += end.Weight;
                Map.Add(path, visited);
                return;
            }

            foreach (var edge in Adjustments[start])
            {
                if (!visited.Contains(edge.End))
                {
                    CalculatePath(edge.End, end, path + start.Weight + edge.End.Weight + edge.Weight, visited);
                }

                visited = Remove(visited, start.Id);
            }
        }

        private List<Vertex> Remove(List<Vertex> visited, int id)
        {
            var result = new List<Vertex>();
            foreach (var e in visited)
            {
                if (e.Id <= id)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        public List<Vertex> GetTransits(List<Vertex> path, string startName, string endName)
        {
            var start = FindByName(startName);
            var end = FindByName(endName);
            return Vertices.Except(path).ToList();
        }

        /*private List<Vertex> GetLevels(List<Vertex> path, Vertex start, Vertex end)
        {
            var levels = new List<Vertex>();
            var visited = new List<Vertex>();
            var current = start;
            foreach (var edge in Adjustments[current])
            {
                var level = GetLevel(current);
                foreach (var e in level)
                    if (!visited.Contains(e) && !path.Contains(e)) levels.Add(e);
                current = edge.End;
            }

            return levels;
        }*/

        public void Print()
        {
            Console.WriteLine($"Vertices: {Vertices.Count}");
            foreach (var vertex in Vertices)
            {
                Console.WriteLine($"Vertex Name: {vertex.Name, 2}, Vertex Weight: {vertex.Weight, 2}");
            }

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine($"Edges: {Edges.Count}");
            foreach (var vertex in Vertices)
            {
                var visited = new List<Vertex>();
                foreach (var edge in Adjustments[vertex])
                {
                    if (visited.Contains(edge.Start) && visited.Contains(edge.End)) continue;

                    Console.WriteLine($"Edge start: {edge.Start.Name, 2}, Edge end: {edge.End.Name,2}, Edge weight: {edge.Weight, 2}");
                    visited.Add(edge.Start);
                    visited.Add(edge.End);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Graph test = new Graph();
            Vertex v1 = new Vertex("v1", 5, 2);
            Vertex v2 = new Vertex("v2", 4, 2);
            Vertex v3 = new Vertex("v3", 6, 8);
            Vertex v4 = new Vertex("v4", 2, 7);
            Vertex v5 = new Vertex("v5", 6, 2);
            Vertex v6 = new Vertex("v6", 2, 1);
            Vertex v7 = new Vertex("v7", 7, 3);
            test.AddNode(v2);
            test.AddNode(v1);
            test.AddNode(v3);
            test.AddNode(v4);
            test.AddNode(v5);
            test.AddNode(v6);
            test.AddNode(v7);
            test.AddEdge(v1, v2, 2);
            test.AddEdge(v1, v3, 4);
            test.AddEdge(v1, v4, 2);
            test.AddEdge(v2, v5, 5);
            test.AddEdge(v3, v6, 9);
            test.AddEdge(v4, v7, 1);
            test.AddEdge(v5, v7, 1);
            test.AddEdge(v6, v7, 7);
            test.Print();

            Console.WriteLine("---------------------------------------------------");
            Console.Write("Critical path: ");
            var critical = test.GetCriticalPath("v1", "v7");
            for (int i = 0; i < critical.Count; i++)
            {
                Console.Write(critical[i].Name + " -> ");
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");

            Console.Write("Transit Nodes: ");
            foreach (var transit in test.GetTransits(critical, "v1", "v7"))
            {
                Console.Write(transit.Name + ",");
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");

        }
    }
}
