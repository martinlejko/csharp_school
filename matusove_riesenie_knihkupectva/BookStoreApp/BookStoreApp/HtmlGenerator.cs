using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
    
namespace NezarkaBookstore 
{ 
    public class HtmlGenerator
    {
        private ModelStore store;
        private TextReader reader;
        private Processor processor;
        private string url = "http://www.nezarka.net/";

        public HtmlGenerator(ModelStore? Store, Processor? processor, TextReader Reader)
        {
            this.store = Store;
            this.reader = Reader;
            this.processor = processor;
        }

        private void CheckInitialSetupInput(string[] urlParams, int customerId)
        {
            switch (urlParams[0])
            {
                case "Books":
                    HandleBooksRequest(urlParams, customerId);
                    break;
                case "ShoppingCart":
                    HandleShoppingCartRequest(urlParams, customerId);
                    break;
                default:
                    GenerateInvalidRequestPage();
                    break;
            }
        }

        private void HandleBooksRequest(string[] urlParams, int customerId)
        {
            if (urlParams.Length == 1)
            {
                GenerateBooksListPage(customerId);
            }
            else if (urlParams.Length == 3 && urlParams[1] == "Detail" && int.TryParse(urlParams[2], out int bookId) && bookId > 0)
            {
                GenerateBooksDetailPage(customerId, bookId);
            }
            else
            {
                GenerateInvalidRequestPage();
            }
        }

        private void HandleShoppingCartRequest(string[] urlParams, int customerId)
        {
            if (urlParams.Length == 1)
            {
                CallCart(customerId);
            }
            else if (urlParams.Length == 3 && (urlParams[1] == "Add" || urlParams[1] == "Remove") && int.TryParse(urlParams[2], out int bookId) && bookId > 0)
            {
                processor.UpdateShoppingCart(urlParams[1], customerId, bookId);
                CallCart(customerId);
            }
            else
            {
                GenerateInvalidRequestPage();
            }
        }

        public void ProcessRequest()
        {
            while (true)
            {
                try
                {
                    string line = reader.ReadLine();
                    if (line == null) break;

                    string[] tokens = line.Split(' ');
                    ValidateRequest(tokens);

                    int customerId = int.Parse(tokens[1]);
                    string slicedUrl = tokens[2].Substring(url.Length);
                    CheckInitialSetupInput(slicedUrl.Split('/'), customerId);
                }
                catch (Exception)
                {
                    GenerateInvalidRequestPage();
                }
            }
        }

        private void ValidateRequest(string[] tokens)
        {
            if (tokens.Length != 3 || tokens[0] != "GET" || !tokens[2].StartsWith(url) || !int.TryParse(tokens[1], out int customerId) || store.GetCustomer(customerId) == null)
            {
                throw new Exception();
            }
        }



        void Ending()
        {
            Console.WriteLine("====");
        }

        public void CallHeader(int customerId)
        {
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
            Console.WriteLine("\t" + store.GetCustomer(customerId).FirstName + ", here is your menu:");
            Console.WriteLine("\t<table>");
            Console.WriteLine("\t\t<tr>");
            Console.WriteLine("\t\t\t<td><a href=\"/Books\">Books</a></td>");
            Console.WriteLine("\t\t\t<td><a href=\"/ShoppingCart\">Cart (" + store.GetNumberOfItems(customerId) + ")</a></td>");
            Console.WriteLine("\t\t</tr>");
            Console.WriteLine("\t</table>");
        }

        private void printBookList()
        {
            int BooksPerRow = 3;
            int BookCounter = 0;
            var bookList = store.GetBooks();
            foreach (var Book in bookList)
            {
                if (BookCounter % BooksPerRow == 0)
                {
                    Console.WriteLine("\t\t<tr>");
                }
                Console.WriteLine("\t\t\t<td style=\"padding: 10px;\">");
                Console.WriteLine("\t\t\t\t<a href=\"/Books/Detail/" + Book.Id + "\">" + Book.Title + "</a><br />");
                Console.WriteLine("\t\t\t\tAuthor: " + Book.Author + "<br />");
                Console.WriteLine("\t\t\t\tPrice: " + Book.Price + " EUR &lt;<a href=\"/ShoppingCart/Add/" + Book.Id + "\">Buy</a>&gt;");
                Console.WriteLine("\t\t\t</td>");
                BookCounter++;

                if (BookCounter % BooksPerRow == 0)
                {
                    Console.WriteLine("\t\t</tr>");
                }
            }
            if (BookCounter % BooksPerRow != 0)
            {
                Console.WriteLine("\t\t</tr>");
            }
        }

        private void GenerateBooksListPage(int customerId)
        {
            CallHeader(customerId);

            Console.WriteLine("\tOur books for you:");
            Console.WriteLine("\t<table>");

            printBookList();

            Console.WriteLine("\t</table>");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Ending();

        }

        private void GenerateBooksDetailPage(int customerId, int bookId)
        {
            CallHeader(customerId);
            var Book = store.GetBook(bookId);

            Console.WriteLine("\tBook details:");
            Console.WriteLine("\t<h2>" + Book.Title + "</h2>");
            Console.WriteLine("\t<p style=\"margin-left: 20px\">");
            Console.WriteLine("\tAuthor: " + Book.Author + "<br />");
            Console.WriteLine("\tPrice: " + Book.Price + " EUR<br />");
            Console.WriteLine("\t</p>");
            Console.WriteLine("\t<h3>&lt;<a href=\"/ShoppingCart/Add/" + Book.Id + "\">Buy this book</a>&gt;</h3>");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Ending();
        }

        private void CallCart(int customerId)
        {
            CallHeader(customerId);
            if (store.GetNumberOfItems(customerId) > 0) { CallCartWithItems(customerId); }
            else { CallEmtyCart(); }
        }

        private void CallCartWithItems(int customerID)
        {
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

        private void PrintCartItems(int customerID)
        {
            var cart = store.GetCustomer(customerID).ShoppingCart;
            var totalPrice = 0m;
            foreach (var item in cart.Items)
            {
                var book = store.GetBook(item.BookId);
                var itemPrice = item.Count * book.Price;
                totalPrice += itemPrice;
                Console.WriteLine("\t\t<tr>");
                Console.WriteLine("\t\t\t<td><a href=\"/Books/Detail/" + book.Id + "\">" + book.Title + "</a></td>");
                Console.WriteLine("\t\t\t<td>" + item.Count + "</td>");
                if (item.Count > 1)
                {
                    Console.WriteLine("\t\t\t<td>" + item.Count + " * " + book.Price + " = " + (itemPrice) + " EUR</td>");
                }
                else
                {
                    Console.WriteLine("\t\t\t<td>" + book.Price + " EUR</td>");
                }
                Console.WriteLine("\t\t\t<td>&lt;<a href=\"/ShoppingCart/Remove/" + book.Id + "\">Remove</a>&gt;</td>");
                Console.WriteLine("\t\t</tr>");
            }
            Console.WriteLine("\t</table>");
            Console.WriteLine("\tTotal price of all items: " + totalPrice + " EUR");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Ending();

        }
        private void CallEmtyCart()
        {
            Console.WriteLine("\tYour shopping cart is EMPTY.");
            Console.WriteLine("</body>");
            Console.WriteLine("</html>");
            Ending();
        }

        public void GenerateInvalidRequestPage()
        {
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
            Ending();
        }
    }
}