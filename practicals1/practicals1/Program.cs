using System.Numerics;
using System.Text;
using System;
using System.IO;

namespace WordCounterProject {
	public class Program {
		static void Main(string[] args) {
			var state = new ProgramInputOutputState();
			if (!state.InitializeFromCommandLineArgs(args)) {
				return;
			}

			var counter = new WordCounter(state.Reader!, state.Writer!);
			counter.Execute();

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
			} catch (UnauthorizedAccessException) {
				Console.WriteLine(FileErrorMessage);
			} catch (ArgumentException) {
				Console.WriteLine(FileErrorMessage);
			} 

			try {
				Writer = new StreamWriter(args[1]);
			} catch (IOException) {
				Console.WriteLine(FileErrorMessage);
			} catch (UnauthorizedAccessException) {
				Console.WriteLine(FileErrorMessage);
			} catch (ArgumentException) {
				Console.WriteLine(FileErrorMessage);
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
				if (!Char.IsWhiteSpace(character)) {
					word.Append(charValue);
				} else {
					if (word.Length > 0) {
						wordCount++;
						word.Clear();
					}
				}
			}
			if (word.Length > 0) {
            wordCount++;}
			
			_writer.WriteLine(wordCount);
		}
	}
}
