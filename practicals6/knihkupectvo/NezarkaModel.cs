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

namespace NezarkaBookstore
{
	class Porgram{

		static void Main(string[] args){
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
		
		private void UrlTokensCheck(string[] UrlTokens){
			if (UrlTokens[0] == "Books"){
				if (UrlTokens.Length == 1){
					Console.WriteLine("ONLY BOOKS");
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
						UrlTokensCheck(slicedUrlTokens);

						// Console.WriteLine("moze byt spracovane " + slicedUrl);

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
	}
}