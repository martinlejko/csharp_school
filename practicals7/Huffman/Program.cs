using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Huffman
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                return;
            }

            Stream reader;
            try
            {
                reader = new FileStream(args[0], FileMode.Open, FileAccess.Read);
            }
            catch (Exception)
            {
                Console.WriteLine("File Error");
                return;
            }

        
            var frequencies = CalculateFrequencies(reader);
            var tree = new HuffmanTree(frequencies);
            tree.PrintTree();
        
        }

        static IEnumerable<KeyValuePair<int, int>> CalculateFrequencies(Stream stream)
        {
            var frequencies = new Dictionary<int, int>();

            int byteValue;
            while ((byteValue = stream.ReadByte()) != -1)
            {
                if (frequencies.ContainsKey(byteValue))
                    frequencies[byteValue]++;
                else
                    frequencies[byteValue] = 1;
            }

            return frequencies.OrderBy(pair => pair.Key);
        }
    }

    public class Node
    {
        public int Weight { get; set; }
        public int? Symbol { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }

    public class HuffmanTree
    {
        public Node Root { get; private set; }

        public HuffmanTree(IEnumerable<KeyValuePair<int, int>> frequencies)
        {
            BuildTree(frequencies);
        }

        private void BuildTree(IEnumerable<KeyValuePair<int, int>> frequencies)
        {
            var nodes = frequencies
                .Select(pair => new Node { Symbol = pair.Key, Weight = pair.Value })
                .OrderBy(node => node.Weight)
                .ToList();

            while (nodes.Count > 1)
            {
                var newNode = new Node
                {
                    Weight = nodes[0].Weight + nodes[1].Weight,
                    Left = nodes[0],
                    Right = nodes[1]
                };

                nodes.RemoveAt(0);
                nodes.RemoveAt(0);

                nodes.Add(newNode);
                nodes = nodes.OrderBy(node => node.Weight).ToList();
            }
            Root = nodes[0];
        }

        public void PrintTree()
        {
            PrintNode(Root);
            Console.WriteLine();
        }

        private void PrintNode(Node node)
        {
            if (node.Symbol.HasValue)
                Console.Write($"*{node.Symbol}:{node.Weight} ");
            else
            {
                Console.Write($"{node.Weight} ");
                PrintNode(node.Left);
                PrintNode(node.Right);
            }
        }
    }
}
