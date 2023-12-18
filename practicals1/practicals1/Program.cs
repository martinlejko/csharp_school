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

using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace WordCounterProject {
	public class Porgram{

		static void Main(){
			ModelStore bookstoreModel;
			View bookstoreView;
			StreamReader reader = new StreamReader("data.txt");
			bookstoreModel = ModelStore.LoadFrom(reader);
			bookstoreView = new View(bookstoreModel, reader);
			bookstoreView.Process();
		}
	}


	//
	// Model
	//

	class ModelStore {
		private List<Book> books = new List<Book>();
		private List<Customer> customers = new List<Customer>();

		public IList<Book> GetBooks() {
			return books;
		}

		public Book GetBook(int id) {
			return books.Find(b => b.Id == id);
		}

		public Customer GetCustomer(int id) {
			return customers.Find(c => c.Id == id);
		}

		public int GetNumberOfItems(int id)
		{
			Customer customer = GetCustomer(id);

			if (customer != null)
			{
				ShoppingCart cart = customer.ShoppingCart;

				if (cart != null && cart.Items != null && cart.Items.Count > 0) {	
					int totalItems = 0;
			
					foreach (var item in cart.Items){
    		            totalItems += item.Count;
      			    }
					return totalItems;
				}
			}
			return 0;
		}

		private bool cointainsCharacter(string str){
			char[] specialChars = { ';', '\n', '\r' }; 

    		foreach (char c in specialChars)
    		{
        		if (str.Contains(c)){
            		return true;
        		}
    		}
    		return false;
		}
		void CheckCorrectFormatBook(string[] tokens){
			string errorMessage = "Data error.";
			string id = tokens[1];
			string title = tokens[2];
			string author = tokens[3];
			string price = tokens[4];
			try{
				if (!int.TryParse(id, out int parsedId) || parsedId < 0){
					throw new Exception();
				}
				if (!int.TryParse(price, out int parsedPrice) || parsedPrice < 0){
					throw new Exception();
				}
				if (cointainsCharacter(title) || cointainsCharacter(author)){
					throw new Exception();
				}

			} catch (Exception){
				Console.WriteLine(errorMessage);
				System.Environment.Exit(0);	
			}
		}

		void CheckCorrectFormatCustomer(string[] tokens){
			string errorMessage = "Data error.";
			string id = tokens[1];
			string name = tokens[2];
			string surname = tokens[3];
			try{
				if (tokens.Length != 4){
					throw new Exception();
				}
				if (!int.TryParse(id, out int parsedId) || parsedId < 0){
					throw new Exception();
				}
				if (cointainsCharacter(name) || cointainsCharacter(surname)){
					throw new Exception();
				}
			} catch (Exception){
				Console.WriteLine(errorMessage);
				System.Environment.Exit(0);	
			}
		}

		void CheckCorrectFormatCart(string[] tokens){
			string errorMessage = "Data error.";
			string customerId = tokens[1];
			string bookId = tokens[2];
			string count = tokens[3];
			try{
				if(!int.TryParse(customerId, out int parsedCustomerId) || parsedCustomerId < 0){
					throw new Exception();
				}
				if(!int.TryParse(bookId, out int parsedBookId) || parsedBookId < 0){
					throw new Exception();
				}
				if(!int.TryParse(count, out int parsedCount) || parsedCount < 0){
					throw new Exception();
				}

			} catch (Exception){
				Console.WriteLine(errorMessage);
				System.Environment.Exit(0);	
			}
		}

		public static ModelStore LoadFrom(TextReader reader) {
			var store = new ModelStore();

			try {
				if (reader.ReadLine() != "DATA-BEGIN") {
					return null;
				}
				while (true) {
					string line = reader.ReadLine();
					if (line == null) {
						return null;
					} else if (line == "DATA-END") {
						break;
					}

					string[] tokens = line.Split(';');
					switch (tokens[0]) {
						case "BOOK":
							store.CheckCorrectFormatBook(tokens);
							store.books.Add(new Book {
								Id = int.Parse(tokens[1]), Title = tokens[2], Author = tokens[3], Price = decimal.Parse(tokens[4])
							});
							break;
						case "CUSTOMER":
							store.CheckCorrectFormatCustomer(tokens);
							store.customers.Add(new Customer {
								Id = int.Parse(tokens[1]), FirstName = tokens[2], LastName = tokens[3]
							});
							break;
						case "CART-ITEM":
							store.CheckCorrectFormatCart(tokens);
							var customer = store.GetCustomer(int.Parse(tokens[1]));
							if (customer == null) {
								return null;
							}
							customer.ShoppingCart.Items.Add(new ShoppingCartItem {
								BookId = int.Parse(tokens[2]), Count = int.Parse(tokens[3])
							});
							break;
						default:
							return null;
					}
				}
			} catch (Exception ex) {
				if (ex is FormatException || ex is IndexOutOfRangeException) {
					return null;
				}
				throw;
			}

			return store;
		}
	}

	class Book {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public decimal Price { get; set; }
	}

	class Customer {
		private ShoppingCart shoppingCart;

		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public ShoppingCart ShoppingCart {
			get {
				if (shoppingCart == null) {
					shoppingCart = new ShoppingCart();
				}
				return shoppingCart;
			}
			set {
				shoppingCart = value;
			}
		}
	}

	class ShoppingCartItem {
		public int BookId { get; set; }
		public int Count { get; set; }
	}

	class ShoppingCart {
		public int CustomerId { get; set; }
		public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();
	}


	//Controller
	class Controller{
		private ModelStore model;
		private TextReader reader;

		public Controller(ModelStore model){
			this.model = model;
		}

		public void Process(){

		}
		
    }

	

	//View
	class View{
		private ModelStore model;
		private TextReader reader;
		private string url = "http://www.nezarka.net/";

		public View(ModelStore model, TextReader reader){
			this.model = model;
			this.reader = reader;
		}
		
		private void UrlTokensCheck(string[] UrlTokens, int customerId){
			if (UrlTokens[0] == "Books"){
				if (UrlTokens.Length == 1){
					CallBooks(customerId);
				} else if (UrlTokens.Length == 3 && UrlTokens[1] == "Detail" && int.TryParse(UrlTokens[2], out int parsedId) && parsedId > 0){

					Console.WriteLine("BOOK DETAIL" + parsedId);
				} else {
					CallInvalidRequest();
				}
			} else if (UrlTokens[0] == "ShoppingCart"){
				if (UrlTokens.Length == 1){
					Console.WriteLine("SHOPPING CART");
				} else if (UrlTokens.Length == 3 && (UrlTokens[1] == "Add" || UrlTokens[1] == "Remove") && int.TryParse(UrlTokens[2], out int parsedId) && parsedId > 0){
					Console.WriteLine("ORDER " + "Action: " + UrlTokens[1] + " BookId: " + parsedId );
				} else {
					CallInvalidRequest();
				}
			} else {
				CallInvalidRequest();
			}
		}

		public void Process(){
				while(true){
					try{
						string line = reader.ReadLine();
						if (line == null){
							break;
						}
						string[] tokens = line.Split(' ');
				
						//check first two parameters
						if (tokens.Length != 3 || tokens[0] != "GET"){
							throw new Exception();
						}

						int customerId = int.Parse(tokens[1]);
						if (customerId < 0){
							throw new Exception();
						}
						if (!tokens[2].StartsWith(url)){
							throw new Exception();
						}

						string slicedUrl = tokens[2].Substring(url.Length);
						string[] slicedUrlTokens = slicedUrl.Split('/');
						UrlTokensCheck(slicedUrlTokens, customerId);

					} catch (Exception){
						CallInvalidRequest();
					}
				}
		}	


		void PrintEnding(){
			Console.WriteLine("====");
		}

		private void CallInvalidRequest(){
    		Console.WriteLine("<!DOCTYPE html>");
			Console.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			Console.WriteLine("<head>");
			Console.WriteLine("\t<meta charset=\"utf-8\" />");
			Console.WriteLine("\t<title>Nezarka.net: Online Shopping for Books</title>");
			Console.WriteLine("</head>");
			Console.WriteLine("<body>");
			Console.WriteLine("<p>Invalid request.</p>");
			Console.WriteLine("</body>");
			Console.WriteLine("</html>");
			PrintEnding();
		}

		private void CallHeader(){
			Console.WriteLine("<!DOCTYPE html>");
			Console.WriteLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
			Console.WriteLine("<head>");
			Console.WriteLine("\t<meta charset=\"utf-8\" />");
			Console.WriteLine("\t<title>Nezarka.net: Online Shopping for Books</title>");
			Console.WriteLine("</head>");
			Console.WriteLine("<body>");
			Console.WriteLine("\t<style type=\"text/css\">");
			Console.WriteLine("\t\ttable, th, td {");
			Console.WriteLine("\t\t\tborder: 1px solid black;");
			Console.WriteLine("\t\t\tborder-collapse: collapse;");
			Console.WriteLine("\t\t}");
			Console.WriteLine("\t\ttable {");
			Console.WriteLine("\t\t\tmargin-bottom: 10px;");
			Console.WriteLine("\t\t}");
			Console.WriteLine("\t\tpre {");
			Console.WriteLine("\t\t\tline-height: 70%;");
			Console.WriteLine("\t\t}");
			Console.WriteLine("\t</style>");
			Console.WriteLine("\t<h1><pre>  v,<br />Nezarka.NET: Online Shopping for Books</pre></h1>");
		}

		private void printBookList(){
			int BooksPerRow = 3;
			int BookCounter = 0;
			var bookList = model.GetBooks();
			
			
		}

		private void CallBooks(int customerId){
			CallHeader();
			Console.WriteLine("\t"+model.GetCustomer(customerId).FirstName + ", here is your menu:");
			//Cart
			Console.WriteLine("\t<table>");
			Console.WriteLine("\t\t<tr>");
			Console.WriteLine("\t\t\t<td><a href=\"/Books\">Books</a></td>");
			Console.WriteLine("\t\t\t<td><a href=\"/ShoppingCart\">Cart (" + model.GetNumberOfItems(customerId) + ")</a></td>");
			Console.WriteLine("\t\t</tr>");
			Console.WriteLine("\t</table>");
			Console.WriteLine("\tOur books for you:");
			Console.WriteLine("\t<table>");


			Console.WriteLine("\t</table>");



			Console.WriteLine("</body>");
			Console.WriteLine("</html>");

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
