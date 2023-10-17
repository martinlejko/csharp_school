#nullable enable
using System.Numerics;
using System.Text;
using System;
using System.IO;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace WordCounterProject {
	public class Program {
		static void Main(string[] args) {
			var state = new ProgramInputOutputState();
			if (!state.InitializeFromCommandLineArgs(args)) {
				return;
			}

			var counter = new WordCounter(state.Reader!, state.Writer!);
			counter.Execute();
			counter.WriteResults();
			state.Dispose();
		}
	}

	public class ProgramInputOutputState : IDisposable {
		public const string ArgumentErrorMessage = "Argument Error";
		public const string FileErrorMessage = "File Error";

		public TextReader? Reader { get; private set; }
		public TextWriter? Writer { get; private set; }

		public bool InitializeFromCommandLineArgs(string[] args) {
			if (args.Length < 2) {
				Console.WriteLine(ArgumentErrorMessage);
				return false;
			}

			try {
				Reader = new StreamReader(args[0]);
			} catch (IOException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			} catch (UnauthorizedAccessException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			} catch (ArgumentException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			} 

			try {
				Writer = new StreamWriter(args[1]);
			} catch (IOException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			} catch (UnauthorizedAccessException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			} catch (ArgumentException) {
				Console.WriteLine(FileErrorMessage);
				return false;
			}

			return true;
		}

		public void Dispose() {
			Reader?.Dispose();
			Writer?.Dispose();
		}
	}

	public class WordCounter {
		private TextReader _reader;
		private TextWriter _writer;
		private List<int> _paragraphWordCount = new List<int>();

		public WordCounter(TextReader reader, TextWriter writer) {
			_reader = reader;
			_writer = writer;
		}

		public void Execute() {
			StringBuilder word = new StringBuilder();
			int wordCount = 0;


			int charValue;
			while ((charValue = _reader.Read()) != -1) {
				char character = (char)charValue;
				if (!char.IsWhiteSpace(character)) {
					word.Append(character);
				} else {
					if (word.Length > 0) {
						wordCount++;
					}
					word.Clear();
					if (character == '\n' && (_reader.Peek() == '\n' || _reader.Peek() == -1)) {
						if ( wordCount != 0) {
							_paragraphWordCount.Add(wordCount);
							wordCount = 0;
						}
					}
				}
			}
		}	
		
		public	void WriteResults() {
			foreach (int num in _paragraphWordCount){
				_writer.WriteLine(num);
			}
		}
	}
}
#nullable disable
