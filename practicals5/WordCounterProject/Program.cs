#nullable enable
using System.Numerics;
using System;
using System.Text;
using System.IO;
using Microsoft.VisualBasic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace WordCounterProject {
		public class Program {
			static void Main(string[] args) {
				var state = new ProgramInputOutputState();
				if (!state.InitializeFromCommandLineArgs(args)) {
					return;
				}

				var processor = new LineProcessor(state.Reader!);
				var printer = new ToSumOrNotToSum(state.Writer!);
				processor.MyReadLines();
				printer.SumCollumn(args[2], processor._table);

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

	public class LineProcessor{
		private TextReader _reader;
		private List<int> _paragraphWordCount = new List<int>();
        private bool _isHeaderCreated = false;
		public LineProcessor(TextReader reader) {
			_reader = reader;
		}
        public Dictionary<string, List<string>> _table = new Dictionary<string, List<string>>();
        public List<string> keyOrder = new List<string>();

		public Dictionary<string, List<string>> getTable(){
			return _table;
		}

	public void MyReadLines() {
			StringBuilder word = new StringBuilder();
			var lineWords = new List<string>();

			int charValue;
			while ((charValue = _reader.Read()) != -1) {
				char character = (char)charValue;
				if (!char.IsWhiteSpace(character)) {
					word.Append(character);
				} else {
					if (word.Length > 0) {
                        lineWords.Add(word.ToString());
                    }
					word.Clear();
					if (character == '\n' || _reader.Peek() == -1) {
						if (lineWords.Count != 0) {
                            ProcessLine(lineWords);
                            lineWords.Clear();
                        }
					}
				}
			}
        }

        public void ProcessLine(List<string> lineWords){
            if (_isHeaderCreated == false){
                _isHeaderCreated = true;
                keyOrder = new List<string>(lineWords);
                foreach (var key in lineWords){
                    _table.Add(key, new List<string>());
                }
                return;
            } else {
                int i = 0;
                foreach (var key in keyOrder){
                    _table[key].Add(lineWords[i]);
                    i++;
                }
            }
        }
	
	}
    public class ToSumOrNotToSum{
		private TextWriter _writer;
		public ToSumOrNotToSum(TextWriter writer) {
			_writer = writer;
		}

        public void SumCollumn(string collumnName, Dictionary<string, List<string>> _table){
            if (_table.ContainsKey(collumnName)){
				long sum = 0;
                try {
                    foreach (var value in _table[collumnName]){
                        sum += long.Parse(value);
                    }
                    _writer.WriteLine(collumnName);
                    _writer.WriteLine(new string('-', collumnName.Length));
                    _writer.WriteLine(sum);
                } catch (FormatException) {
                    Console.WriteLine("Invalid Integer Value");
                    return;
                }
             } else {
				Console.WriteLine("Non-existent Column Name");
			}
        }

    }
}
#nullable disable


// #nullable enable
// using System.Numerics;
// using System;
// using System.Text;
// using System.IO;
// using Microsoft.VisualBasic;
// using System.Security.Cryptography.X509Certificates;
// using System.Net.Http.Headers;
// using System.Collections.Generic;
// using System.Runtime.InteropServices;
// using System.Collections.Specialized;
// using System.Reflection.Metadata;

// namespace WordCounterProject {
// 		public class Program {
// 			static void Main(string[] args) {
// 				var state = new ProgramInputOutputState();
// 				if (!state.InitializeFromCommandLineArgs(args)) {
// 					return;
// 				}

// 				var processor = new LineProcessor(state.Reader!);
// 				var printer = new ToSumOrNotToSum(state.Writer!);
// 				processor.MyReadLines();
// 				printer.SumCollumn(args[2], processor.getTable());

// 				state.Dispose();
// 			}
// 		}

// 	public class ProgramInputOutputState : IDisposable {
// 		public const string ArgumentErrorMessage = "Argument Error";
// 		public const string FileErrorMessage = "File Error";

// 		public TextReader? Reader { get; private set; }
// 		public TextWriter? Writer { get; private set; }

// 		public bool InitializeFromCommandLineArgs(string[] args) {
// 			if (args.Length < 2) {
// 				Console.WriteLine(ArgumentErrorMessage);
// 				return false;
// 			}

// 			try {
// 				Reader = new StreamReader(args[0]);
// 			} catch (IOException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			} catch (UnauthorizedAccessException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			} catch (ArgumentException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			} 

// 			try {
// 				Writer = new StreamWriter(args[1]);
// 			} catch (IOException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			} catch (UnauthorizedAccessException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			} catch (ArgumentException) {
// 				Console.WriteLine(FileErrorMessage);
// 				return false;
// 			}

//             return true;
// 		}

// 		public void Dispose() {
// 			Reader?.Dispose();
// 			Writer?.Dispose();
// 		}
// 	}

// 	public class LineProcessor{
// 		private TextReader _reader;
// 		private List<int> _paragraphWordCount = new List<int>();
//         private bool _isHeaderCreated = false;
//         private int? lineLength;
// 		public LineProcessor(TextReader reader) {
// 			_reader = reader;
// 		}
//         public Dictionary<string, List<string>> _table = new Dictionary<string, List<string>>();
//         public List<string> keyOrder = new List<string>();

// 		public Dictionary<string, List<string>> getTable(){
// 			return _table;
// 		}

// 	public void MyReadLines() {
// 			StringBuilder word = new StringBuilder();
// 			var lineWords = new List<string>();

// 			int charValue;
// 			while ((charValue = _reader.Read()) != -1) {
// 				char character = (char)charValue;
// 				if (!char.IsWhiteSpace(character) && _reader.Peek() != -1) {
// 					word.Append(character);
// 				} else {
// 					if (word.Length > 0) {
//                         lineWords.Add(word.ToString());
//                     }
// 					word.Clear();
// 					if (character == '\n' || _reader.Peek() == -1) {
// 						if (lineWords.Count != 0) {
//                             if(lineLength != null && lineWords.Count != lineLength){
//                                 Console.WriteLine("Invalid File Format");
//                                 return;
//                             }
//                             ProcessLine(lineWords);
//                             lineWords.Clear();
//                         }
// 					}
// 				}
// 			}
//         }

//         public void ProcessLine(List<string> lineWords){
//             if (_isHeaderCreated == false){
//                 _isHeaderCreated = true;
//                 keyOrder = new List<string>(lineWords);
//                 lineLength = lineWords.Count;
//                 foreach (var key in lineWords){
//                     if (_table.ContainsKey(key)){
//                         string newKey = key;
//                         int counter = 1;
//                         while (_table.ContainsKey(newKey))
//                         {
//                             newKey = key + counter.ToString();
//                             counter++;
//                         }
//                         _table.Add(newKey, new List<string>());
//                         continue;
//                     }
//                     _table.Add(key, new List<string>());
//                 }
//                 return;
//             } else {
//                 int i = 0;
//                 foreach (var key in keyOrder){
//                     _table[key].Add(lineWords[i]);
//                     i++;
//                 }
//             }
//         }
	
// 	}
//     public class ToSumOrNotToSum{
// 		private TextWriter _writer;
// 		public ToSumOrNotToSum(TextWriter writer) {
// 			_writer = writer;
// 		}

//         public void SumCollumn(string collumnName, Dictionary<string, List<string>> _table){
//             if (_table.ContainsKey(collumnName)){
// 				long sum = 0;
//                 try {
//                     foreach (var value in _table[collumnName]){
//                         sum += long.Parse(value);
//                     }
//                     _writer.WriteLine(collumnName);
//                     _writer.WriteLine(new string('-', collumnName.Length));
//                     _writer.WriteLine(sum);
//                 } catch (FormatException) {
//                     Console.WriteLine("Invalid Integer Value");
//                     return;
//                 }
//              } else {
//                 if (_table.Count == 0){
//                     Console.WriteLine("Invalid File Format");
//                     return;
//                 }
// 				Console.WriteLine("Non-existent Column Name");
// 			}
//         }

//     }
// }
// #nullable disable