using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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

            var outputStream = new FileStream(args[0] + ".huff", FileMode.Create, FileAccess.Write);
            var writer = new BinaryWriter(outputStream);
            WriteHeader(writer);
            tree.WriteTree(writer);
            WriteFooter(writer);
            tree.GeneratePackedEncodingArray();
            // tree.PrintEncodingArray();
            reader.Seek(0, SeekOrigin.Begin);
            WriteBytesOfSymbols(writer, reader, tree._encodingArray);
            outputStream.Close();

        
        }

        static IEnumerable<KeyValuePair<long, long>> CalculateFrequencies(Stream stream)
        {
            var frequencies = new Dictionary<long, long>();

            long byteValue;
            while ((byteValue = stream.ReadByte()) != -1)
            {
                if (frequencies.ContainsKey(byteValue))
                    frequencies[byteValue]++;
                else
                    frequencies[byteValue] = 1;
            }

            return frequencies.OrderBy(pair => pair.Key);
        }
        public static void WriteHeader(BinaryWriter writer)
        {
            writer.Write(new byte[] { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 });
        }
        public static void WriteFooter(BinaryWriter writer)
        {
            writer.Write(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
        }

        public static void WriteBytesOfSymbols(BinaryWriter outputFile, Stream inputFile, (byte[] encodedSequence, int sequenceLength)[] encodingArray)
        {
            int bitCount = 0; 
            // Console.WriteLine("Encoding:");
            int readByte;
            
            byte buffer = (byte)0;
            int bufferIndex = 0;

            while ((readByte = inputFile.ReadByte()) != -1)
            {
                byte currentSymbol = (byte)readByte;
                var encodingTuple = encodingArray[currentSymbol];
                byte[] encodedSequence = encodingTuple.encodedSequence; 
                int sequenceLength = encodingTuple.sequenceLength; 
                // Console.WriteLine(Convert.ToString(buffer, 2));
                // Console.Write(" ");


                for (int i = 0; i < sequenceLength ; i++)
                {
                    byte currentByte = encodedSequence[i/8];
                    int currentBit = currentByte >> ((i % 8)) & 1;
                    // Console.WriteLine($"curr: {currentBit}");
                    // Console.WriteLine(Convert.ToString(currentByte, 2));
                    buffer |= (byte)(currentBit << (bufferIndex));
                    bufferIndex++;
                    if (bufferIndex == 8)
                    {
                        bufferIndex = 0;
                        outputFile.Write(buffer);
                        buffer = (byte)0;
                    }

                }

            }
            // Console.WriteLine($"bufferIndex: {bufferIndex}");
            // Console.WriteLine(Convert.ToString(buffer, 2));
            if (bufferIndex != 0)
            {
                outputFile.Write(new byte[] {buffer});
            }
        }
    }

    public class Node
    {
        public long Weight { get; set; }
        public long? Symbol { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }

    public class HuffmanTree
    {
        public (byte[] encodedSequence, int sequenceLength)[] _encodingArray;
        public Node Root { get; private set; }

        public HuffmanTree(IEnumerable<KeyValuePair<long, long>> frequencies)
        {
            BuildTree(frequencies);
        }

        private void BuildTree(IEnumerable<KeyValuePair<long, long>> frequencies)
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

        public void WriteTree(BinaryWriter writer)
        {
            WriteNode(writer, Root);
        }

        private void WriteNode(BinaryWriter writer, Node node)
        {
            if (node.Symbol.HasValue)
            {
                ulong value = (ulong)node.Symbol << 56;
                ulong weight = (ulong)node.Weight << 1;
                ulong leaf = (ulong)1;
                ulong outputValue = value | weight | leaf;
                byte[] leafBytes = BitConverter.GetBytes(outputValue);
                writer.Write(leafBytes, 0, leafBytes.Length);
            }
            else
            {
                ulong weight = (ulong)node.Weight << 1;
                ulong outputValue = weight;
                byte[] innerBytes = BitConverter.GetBytes(outputValue);
                writer.Write(innerBytes, 0, innerBytes.Length);

                WriteNode(writer, node.Left);
                WriteNode(writer, node.Right);
            }
        }

        public (byte[], int)[] GeneratePackedEncodingArray()
        {
            _encodingArray = new (byte[], int)[256]; // Array to store encoded sequences

            if (Root != null)
            {
                List<byte> currentBytes = new List<byte>() { 0 };
                TraverseAndGeneratePackedEncodingArray(Root, currentBytes, 0);
            }

            return _encodingArray;
        }

        private void TraverseAndGeneratePackedEncodingArray(Node node, List<byte> currentBytes, int depth)
        {
            if (node.Symbol.HasValue)
            {
                _encodingArray[node.Symbol.Value] = (currentBytes.ToArray(), depth);
            }
            else
            {
                if (depth/8 >= currentBytes.Count)
                {
                    currentBytes.Add(0);
                }
                if (node.Left != null)
                {

                    currentBytes[depth/8] &= (byte)~(1 << (depth%8));
                    TraverseAndGeneratePackedEncodingArray(node.Left, currentBytes, depth + 1);

                }
                if (node.Right != null)
                {

                    currentBytes[depth/8] |= (byte)(1 << (depth%8));
                    TraverseAndGeneratePackedEncodingArray(node.Right, currentBytes, depth + 1);
                }
            }
        }

        public void PrintEncodingArray()
    {
        if (_encodingArray != null)
        {
            for (int i = 0; i < _encodingArray.Length; i++)
            {
                if (_encodingArray[i].encodedSequence != null)
                {
                    Console.Write($"Byte {i}: ");
                    foreach (byte b in _encodingArray[i].encodedSequence)
                    {
                        Console.Write($"{b:X2} "); // Prints the byte in hexadecimal format
                    }
                    Console.WriteLine($"(Length: {_encodingArray[i].sequenceLength})");
                }
            }
        }
    }
    }
}
