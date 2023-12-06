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
using System.Security.Cryptography;

namespace NezarkaBookstore
{
	public class Porgram{

		public static void Main(){
			ModelStore bookstoreModel;
			View bookstoreView;
			Controller controller;
			StreamReader reader = new StreamReader(Console.OpenStandardInput());
			bookstoreModel = ModelStore.LoadFrom(reader);
			if (bookstoreModel == null){
				Console.WriteLine("Data error.");
				System.Environment.Exit(0);
			}
			controller = new Controller(bookstoreModel);
			bookstoreView = new View(bookstoreModel, controller, reader);
			bookstoreView.Process();
		}
	}


	//
	// Model
	//

	public class ModelStore {
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

				return cart.Items.Count;
			}
			return 0;
		}

		public int GetTotalPrice(int customerID){
			var cart = GetCustomer(customerID).ShoppingCart;
			int totalPrice = 0;
			foreach (var item in cart.Items){
				var book = GetBook(item.BookId);
				totalPrice += item.Count * (int)book.Price;
			}
			return totalPrice;
		}
		public bool BookExists(int BookId){
			if (GetBook(BookId) == null){
				return false;
			}
			return true;
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

	public class Book {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public decimal Price { get; set; }
	}

	public class Customer {
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

	public class ShoppingCartItem {
		public int BookId { get; set; }
		public int Count { get; set; }
	}

	public class ShoppingCart {
		public int CustomerId { get; set; }
		public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();
	}


	//Controller
	public class Controller{
		private ModelStore model;

		public Controller(ModelStore model){
			this.model = model;
		}

		public void ProcessAction(string action, int customerId, int bookId){
			if (action == "Add"){
				if (model.BookExists(bookId)){
					var customer = model.GetCustomer(customerId);
					var cart = customer.ShoppingCart;
					var item = cart.Items.Find(i => i.BookId == bookId);
					if (item == null){
						cart.Items.Add(new ShoppingCartItem{BookId = bookId, Count = 1});
					} else {
						item.Count++;
					}
				} else {
					throw new Exception();
				}
			}
			if (action == "Remove"){
				var customer = model.GetCustomer(customerId);
				var cart = customer.ShoppingCart;
				var item = cart.Items.Find(i => i.BookId == bookId);
				if (item != null){
					if (item.Count > 1){
						item.Count--;
					} else {
						cart.Items.Remove(item);
					}
				} else {
					throw new Exception();
				}
			}
		}
		
    }

	

	//View
	public class View{
		private ModelStore model;
		private TextReader reader;
		private Controller controller;
		private string url = "http://www.nezarka.net/";

		public View(ModelStore model, Controller controller,TextReader reader){
			this.model = model;
			this.reader = reader;
			this.controller = controller;
		}
		
		private void UrlTokensCheck(string[] UrlTokens, int customerId){
			if (UrlTokens[0] == "Books"){
				if (UrlTokens.Length == 1){
					CallBooks(customerId);
				} else if (UrlTokens.Length == 3 && UrlTokens[1] == "Detail" && int.TryParse(UrlTokens[2], out int parsedId) && parsedId > 0){
					CallBookDetail(customerId, parsedId);
				} else {
					CallInvalidRequest();
				}
			} else if (UrlTokens[0] == "ShoppingCart"){
				if (UrlTokens.Length == 1){
					CallCart(customerId);
				} else if (UrlTokens.Length == 3 && (UrlTokens[1] == "Add" || UrlTokens[1] == "Remove") && int.TryParse(UrlTokens[2], out int parsedId) && parsedId > 0){
					controller.ProcessAction(UrlTokens[1], customerId, parsedId);
					CallCart(customerId);
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
						if (customerId < 0 || model.GetCustomer(customerId) == null){
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

		public void CallInvalidRequest(){
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

		private void CallHeader(int customerId){
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
			Console.WriteLine("\t"+model.GetCustomer(customerId).FirstName + ", here is your menu:");
			Console.WriteLine("\t<table>");
			Console.WriteLine("\t\t<tr>");
			Console.WriteLine("\t\t\t<td><a href=\"/Books\">Books</a></td>");
			Console.WriteLine("\t\t\t<td><a href=\"/ShoppingCart\">Cart (" + model.GetNumberOfItems(customerId) + ")</a></td>");
			Console.WriteLine("\t\t</tr>");
			Console.WriteLine("\t</table>");
		}

		private void printBookList(){
			int BooksPerRow = 3;
			int BookCounter = 0;
			var bookList = model.GetBooks();
			foreach (var Book in bookList){
				if (BookCounter % BooksPerRow == 0){
					Console.WriteLine("\t\t<tr>");
				}
				Console.WriteLine("\t\t\t<td style=\"padding: 10px;\">");
				Console.WriteLine("\t\t\t\t<a href=\"/Books/Detail/"+ Book.Id + "\">" + Book.Title + "</a><br />");
				Console.WriteLine("\t\t\t\tAuthor: " + Book.Author + "<br />");
				Console.WriteLine("\t\t\t\tPrice: " + Book.Price + " EUR &lt;<a href=\"/ShoppingCart/Add/"+ Book.Id + "\">Buy</a>&gt;");
				Console.WriteLine("\t\t\t</td>");
				BookCounter++;

				if (BookCounter % BooksPerRow == 0){
					Console.WriteLine("\t\t</tr>");
				}
			}
			if (BookCounter % BooksPerRow != 0){
				Console.WriteLine("\t\t</tr>");
			}
		}

		private void CallBooks(int customerId){
			CallHeader(customerId);

			Console.WriteLine("\tOur books for you:");
			Console.WriteLine("\t<table>");

			printBookList();

			Console.WriteLine("\t</table>");
			Console.WriteLine("</body>");
			Console.WriteLine("</html>");
			PrintEnding();

		}

		private void CallBookDetail(int customerId, int bookId){
			CallHeader(customerId);
			var Book = model.GetBook(bookId);

			Console.WriteLine("\tBook details:");
			Console.WriteLine("\t<h2>"+ Book.Title +"</h2>");
			Console.WriteLine("\t<p style=\"margin-left: 20px\">");
			Console.WriteLine("\tAuthor: " + Book.Author + "<br />");
			Console.WriteLine("\tPrice: " + Book.Price + " EUR<br />");
			Console.WriteLine("\t</p>");
			Console.WriteLine("\t<h3>&lt;<a href=\"/ShoppingCart/Add/" + Book.Id + "\">Buy this book</a>&gt;</h3>");
			Console.WriteLine("</body>");
			Console.WriteLine("</html>");
			PrintEnding();	
		}

		private void CallCart(int customerId){
			CallHeader(customerId);
			if (model.GetNumberOfItems(customerId) == 0){
				CallEmtyCart();
			} else {
				CallCartWithItems(customerId);
			}
		}

		private void CallCartWithItems(int customerID){
			Console.WriteLine("\tYour shopping cart:");
			Console.WriteLine("\t<table>");
			Console.WriteLine("\t\t<tr>");
			Console.WriteLine("\t\t\t<th>Title</th>");
			Console.WriteLine("\t\t\t<th>Count</th>");
			Console.WriteLine("\t\t\t<th>Price</th>");
			Console.WriteLine("\t\t\t<th>Actions</th>");
			Console.WriteLine("\t\t</tr>");
			PrintCartItems(customerID);

		}

		private void PrintCartItems(int customerID){
			var cart = model.GetCustomer(customerID).ShoppingCart;
			foreach (var item in cart.Items){
				var book = model.GetBook(item.BookId);
				Console.WriteLine("\t\t<tr>");
				Console.WriteLine("\t\t\t<td><a href=\"/Books/Detail/" + book.Id + "\">" + book.Title + "</a></td>");
				Console.WriteLine("\t\t\t<td>" + item.Count + "</td>");
				if (item.Count > 1){
					Console.WriteLine("\t\t\t<td>" + item.Count +" * " + book.Price + " = " + (item.Count * book.Price) + " EUR</td>");
				} else {
					Console.WriteLine("\t\t\t<td>" + book.Price + " EUR</td>");
				}
				Console.WriteLine("\t\t\t<td>&lt;<a href=\"/ShoppingCart/Remove/" + book.Id + "\">Remove</a>&gt;</td>");
				Console.WriteLine("\t\t</tr>");
			}
			Console.WriteLine("\t</table>");
			Console.WriteLine("\tTotal price of all items: " + model.GetTotalPrice(customerID) + " EUR");
			Console.WriteLine("</body>");
			Console.WriteLine("</html>");
			PrintEnding();

		}
		private void CallEmtyCart(){
			Console.WriteLine("\tYour shopping cart is EMPTY.");
			Console.WriteLine("</body>");
			Console.WriteLine("</html>");
			PrintEnding();
		}
	}
}