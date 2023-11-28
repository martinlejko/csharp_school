#nullable enable
using System.Numerics;
using System.Text;
using System;
using System.IO;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Data;

namespace WordCounterProject {
	public class Program {
		static void Main(string[] args) {
			var state = new ProgramInputOutputState();
			if (!state.InitializeFromCommandLineArgs(args)) {
				return;
			}
			var counter = new WordCounter(state.Reader!, state.Writer!, int.Parse(args[^1]), state.highlightSpaces);
			int fileIndex = 1;

			while(args[fileIndex] != args[^2]) {
				counter.readParagraphs();
				counter._reader = new StreamReader(args[fileIndex]);
				fileIndex++;
			}
			state.Dispose();
		}
	}

	public class ProgramInputOutputState : IDisposable {
		public bool highlightSpaces = false;
		public const string ArgumentErrorMessage = "Argument Error";

		public TextReader? Reader { get; private set; }
		public TextWriter? Writer { get; private set; }

		public bool InitializeFromCommandLineArgs(string[] args) {
			if (args[0] == "--highlight-spaces") {
				if (args.Length < 4) {
					Console.WriteLine(ArgumentErrorMessage);
					return false;
                }
                highlightSpaces = true;
				Reader = new StreamReader(args[1]);

			} else {
				if (args.Length < 3) {
					Console.WriteLine(ArgumentErrorMessage);
					return false;
				}
				Reader = new StreamReader(args[0]);
			}


			Writer = new StreamWriter(args[^2]);

			try{
				if(int.Parse(args[^1]) < 1) {
					Console.WriteLine(ArgumentErrorMessage);
					return false;
				}
			} catch (FormatException) {
				Console.WriteLine(ArgumentErrorMessage);
				return false;
			} catch (OverflowException) {
				Console.WriteLine(ArgumentErrorMessage);
				return false;
			}
			return true;
		}

		public void Dispose() {
			Reader?.Dispose();
			Writer?.Dispose();
		}
	}

	public class Token{
		public enum TokenType {
			Word,
			EndOfParagraph,
			EndOfFile
		}
		public TokenType Type;
		public string? Value;
	}

	public class WordCounter {
 		public bool highlightSpaces;
		public TextReader _reader;
		private TextWriter _writer;
		private List<int> _paragraphWordCount = new List<int>();
		private int _lineLength;
		
		public WordCounter(TextReader reader, TextWriter writer, int lineLength, bool highlightSpaces) {
			_reader = reader;
			_writer = writer;
			_lineLength = lineLength;
			this.highlightSpaces = highlightSpaces;
		}

		int wordsLength = 0;
		int wordsCount = 0;
		bool lastParagraph = false;
		bool atlestOneWord = false;
		List<string> words = new List<string>();


		public void readParagraphs() {
			StringBuilder word = new StringBuilder();
			int charValue;
			bool startOfParagraph = false;
			bool previousNewLine = false;
			Token token;

			while ((charValue = _reader.Read()) != -1) {
				char character = (char)charValue;
				if (!char.IsWhiteSpace(character)) {
					word.Append(character);
				} else {
					if (word.Length > 0) {
						token = new Token { Type = Token.TokenType.Word, Value = word.ToString() };
						processLine(token);
						startOfParagraph = true;
						atlestOneWord = true;
						previousNewLine = false;
					}
					if ( startOfParagraph && character == '\n') {
						if (previousNewLine){
							startOfParagraph = false;
							lastParagraph = true;
							token = new Token { Type = Token.TokenType.EndOfParagraph };
							processLine(token);
							
						}
						previousNewLine = true;
					}
					word.Clear();
				}

			}
			if (word.Length > 0) {
				token = new Token { Type = Token.TokenType.Word, Value = word.ToString() };
				processLine(token);
			}
			token = new Token { Type = Token.TokenType.EndOfFile };
			processLine(token);

		void processLine(Token token) {
			if (token.Type == Token.TokenType.Word) {
				if (lastParagraph) {
					if (highlightSpaces) {
						_writer.Write("<-");
						_writer.WriteLine();
						_writer.WriteLine();
					} else{
						_writer.WriteLine();
						_writer.WriteLine();
					}
					lastParagraph = false;
				}

				string word = token.Value!;
				if (wordsLength + word.Length + wordsCount > _lineLength) {
					if (wordsCount != 0) { printLine();}
					wordsLength = 0;
					wordsCount = 0;
					words.Clear();
					wordsLength += word.Length;
					wordsCount++;
					words.Add(word);
				} else {
					wordsLength += word.Length;
					wordsCount++;
					words.Add(word);
				}
			}
			if (token.Type == Token.TokenType.EndOfParagraph) {
				for (int i = 0; i < words.Count; i++) {
					_writer.Write(words[i]);
					if (i != words.Count - 1) {
						if (highlightSpaces) {
							_writer.Write(".");
						} else {
							_writer.Write(" ");
						}
					}
				}

				wordsLength = 0;
				wordsCount = 0;
				words.Clear();	
			}
			if (token.Type == Token.TokenType.EndOfFile) {
				return;
			}


		}
				
        void printLine(){
			int remainingSpaces = _lineLength - wordsLength;
			for (int i = 0; i < words.Count; i++) {
				_writer.Write(words[i]);
				wordsCount--;
				if (wordsCount != 0) {
					int numberOfSpaces = (remainingSpaces + wordsCount -1) / wordsCount ;
					for (int j = 0; j < numberOfSpaces; j++) {
						if (highlightSpaces) {
							_writer.Write(".");
						} else {
							_writer.Write(" ");
						}
					}
					remainingSpaces -= numberOfSpaces;
				}
			}
			if (highlightSpaces){
				_writer.WriteLine("<-");
			}else{
				_writer.WriteLine();
			}
		}

		if (atlestOneWord) {
			if (highlightSpaces){
				_writer.WriteLine("<-");
			}else{
				_writer.WriteLine();
			}
		}

	}
}
}
#nullable disable
