using System;
using System.Collections.Generic;

namespace WordCounterProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<string> words = new List<string> { "A", "b", "c" };
            int index = 0;

            while (index < words.Count){
				Console.WriteLine(words[index++]);
				Console.WriteLine(index);
			} 
            }
        }
    }

