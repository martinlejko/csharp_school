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
			int fileIndex = state.startIndex;

			while(args[fileIndex] != args[^2]) {
				try {
					counter._reader = new StreamReader(args[fileIndex]);
				} catch (FileNotFoundException) {
					fileIndex++;
					continue;
				} catch (DirectoryNotFoundException) {
					fileIndex++;
					continue;
				} catch (IOException) {
					fileIndex++;
					continue;
				} catch (UnauthorizedAccessException) {
					fileIndex++;
					continue;
				}
				counter.readParagraphs();
				fileIndex++;
			}
			counter.processLine(new Token { Type = Token.TokenType.EndOfParagraph });
			
			state.Dispose();
		}
	}


	public class ProgramInputOutputState : IDisposable {
		public bool highlightSpaces = false;
		public int startIndex = 0;
		public const string ArgumentErrorMessage = "Argument Error";

		public TextReader? Reader { get; private set; }
		public TextWriter? Writer { get; private set; }

		public bool InitializeFromCommandLineArgs(string[] args) {
			if (args.Length < 3) {
				Console.WriteLine(ArgumentErrorMessage);
				return false;
			}
			if (args[0] == "--highlight-spaces") {
				if (args.Length < 4) {
					Console.WriteLine(ArgumentErrorMessage);
					return false;
                }
                highlightSpaces = true;
				startIndex = 1;
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

		public int wordsLength = 0;
		public int wordsCount = 0;
		bool lastParagraph = false;
		bool atlestOneWord = false;
		public List<string> words = new List<string>();


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
		}

		
		public void processLine(Token token) {
			if (token.Type == Token.TokenType.Word) {
				if (lastParagraph) {
					if (highlightSpaces) {
						_writer.WriteLine("<-");
					} else{
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
				if (atlestOneWord && words.Count != 0) {
					if (highlightSpaces) {
						_writer.WriteLine("<-");
					} else {
						_writer.WriteLine();
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
				
        public void printLine(){
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


	}
}

#nullable disable


// using System;
// using System.Data;
// using System.IO;
// using Microsoft.VisualStudio.TestPlatform.Utilities;

// namespace TestWordCount;

// public class WordCounterTests
// {
//     [Fact]
//     public void PrintLineNoWordsNoHighlight_Test()
//     {
//         // Arrange
//         const int lineLength = 12;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineNoWordsHighlight_Test()
//     {
//         // Arrange
//         const int lineLength = 12;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "<-\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineOneWordNoHightLight_Test()
//     {
//         // Arrange
//         const int lineLength = 12;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {"word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineOneWordHightLight_Test()
//     {
//         // Arrange
//         const int lineLength = 12;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {"word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength =  wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word<-\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineTwoWordsNoHightLightOneSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 9;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {"word", "word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word word\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineTwoWordsHightLightOneSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 9;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {"word", "word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word.word<-\n";
//         Assert.Equal(expectedOutput, output);
//     }


//     [Fact]
//     public void PrintLineTwoWordsNoHightLightEvenNumberOfSpacesInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 10;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {"word", "word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word  word\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineTwoWordsHightLightEvenNumberOfSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 10;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {"word", "word"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "word..word<-\n";
//         Assert.Equal(expectedOutput, output);
//     }

//         [Fact]
//     public void PrintLineMultipleWordsNoHightLightEvenNumberSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 25;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "Martin  Lejko  CVUT  code\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineMultipleWordsHightLightEvenNumberSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 25;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "Martin..Lejko..CVUT..code<-\n";
//         Assert.Equal(expectedOutput, output);
//     }

//             [Fact]
//     public void PrintLineMultipleWordsNoHightLightOddNumberSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 23;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, false);
//         wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "Martin  Lejko CVUT code\n";
//         Assert.Equal(expectedOutput, output);
//     }

//     [Fact]
//     public void PrintLineMultipleWordsHightLightOddNumberSpaceInBetween_Test()
//     {
//         // Arrange
//         const int lineLength = 23;
//         var input = "";
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var wordCounter = new WordCounter(reader, writer, lineLength, true);
//         wordCounter.words = new List<string> {"Martin", "Lejko", "CVUT", "code"};
//         wordCounter.wordsCount = wordCounter.words.Count;
//         wordCounter.wordsLength = wordCounter.words.Sum(word => word.Length);
//         // Act
//         wordCounter.printLine();
//         var output = writer.ToString();

//         // Assert
//         string expectedOutput = "Martin..Lejko.CVUT.code<-\n";
//         Assert.Equal(expectedOutput, output);
//     }
// }










//EXPLANATION OF THE TESTS
//TIETO TESTY SU INTEGRACNE Z MINULA ALE TAKTIEZ BOLI POUZITE PRI PISANI KODU

// public class WordCounterTests2
// {
//     private void RunWordCounterTest(string input, string expectedOutput, int maxParagraphLength = 0, bool hihglightSpaces = false) {
//         var reader = new StringReader(input);
//         var writer = new StringWriter();
//         var counter = new WordCounter(reader, writer, maxParagraphLength, hihglightSpaces);
//         counter.readParagraphs();
//         counter.processLine(new Token { Type = Token.TokenType.EndOfParagraph });
//         string actualOutput = writer.ToString();
//         Assert.Equal(expectedOutput, actualOutput);
//     }



//     [Fact]
//     public void RecodexTest1() {

//         string input = "If a train station is where the train stops,\nwhat is a work station?";
//         string expectedOutput = "If     a    train\nstation  is where\nthe  train stops,\nwhat  is  a  work\nstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }
//         [Fact]
//     public void EmptyFileTest() {

//         string input = "";
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void OnlyNewLinesTest() {

//         string input = """
        


//         """;
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void WhiteSpacesLineTest() {

//         string input = """

                                 
//         """;
//         string expectedOutput = "";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//     [Fact]
//     public void BasicOneLine() {

//         string input = """
//         If a train station is where the train stops.
//         """;
//         string expectedOutput = "If a train\nstation is\nwhere  the\ntrain\nstops.\n";
//         RunWordCounterTest(input, expectedOutput, 10);
//     }

//     [Fact]
//     public void WordLongerThenRange() {
//         string input = """
//         dsibagiasdbgibasidgbiabg aigbsaig abgisbg
//         """;
//         string expectedOutput = "dsibagiasdbgibasidgbiabg\naigbsaig\nabgisbg\n";
//         RunWordCounterTest(input, expectedOutput, 5);
//     }

//     [Fact]
//     public void TwoNormalParagraphsTest() {

//         string input = """
//         If a train station is where the train stops.
        
//         what is a workstation?
//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat     is     a\nworkstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }

//     [Fact]
//     public void LeadingAndMultipleNewLinesTest() {

//         string input = """




//         If a train station is where the train stops.
        



//         what is a workstation?



//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat     is     a\nworkstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);
//     }

//    [Fact]
//     public void PerfectFittingTest() {

//         string input = """
//         If a train
//         station is
//         where thed
//         train stop
        
//         what is ae
//         workstatio
//         """;
//         string expectedOutput = "If a train\nstation is\nwhere thed\ntrain stop\n\nwhat is ae\nworkstatio\n";
//         RunWordCounterTest(input, expectedOutput, 10);

//     }

//     [Fact]
//     public void TabsAsWhiteSpacesTest() {

//         string input = """
//         If a train              station is where the train stops.
        
//         what is a                work station?
//         """;
//         string expectedOutput = "If     a    train\nstation  is where\nthe train stops.\n\nwhat  is  a  work\nstation?\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }

//     [Fact]
//     public void NumbersAsNonWhiteTest() {

//         string input = """
//         If a 12345 station 12 where 123 train stops.
//         """;
//         string expectedOutput = "If     a    12345\nstation  12 where\n123 train stops.\n";
//         RunWordCounterTest(input, expectedOutput, 17);

//     }
// }
