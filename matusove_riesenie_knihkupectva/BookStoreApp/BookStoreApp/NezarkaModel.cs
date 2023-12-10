using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace NezarkaBookstore
{

    public class ModelStore
    {
        private List<Book> books = new List<Book>();
        public List<Customer> customers = new List<Customer>();

        public IList<Book> GetBooks()
        {
            return books;
        }

        public Book GetBook(int id)
        {
            return books.Find(b => b.Id == id);
        }

        public Customer GetCustomer(int id)
        {
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

        

        private bool ContainsInvalidChar(string str)
        {
            var invalidCharacters = new HashSet<char> { ';', '\n', '\r' };

            return str.Any(invalidCharacters.Contains);
        }

        public bool ValidateBook(int BookId)
        {
            if (GetBook(BookId) == null)
            {
                return false;
            }
            return true;
        }

        


        void CheckCorrectFormatCustomer(string[] tokens)
        {
            string errorMessage = "Data error.";
            string id = tokens[1];
            string name = tokens[2];
            string surname = tokens[3];
            try
            {
                if (tokens.Length != 4)
                {
                    throw new Exception();
                }
                if (!int.TryParse(id, out int parsedId) || parsedId < 0)
                {
                    throw new Exception();
                }
                if (ContainsInvalidChar(name) || ContainsInvalidChar(surname))
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine(errorMessage);
                System.Environment.Exit(0);
            }
        }

        private void ValidateCartItemFormat(string[] tokens)
        {
            const string errorMessage = "Data error.";
            try
            {
                if (!int.TryParse(tokens[1], out int parsedCustomerId) || parsedCustomerId < 0)
                    throw new FormatException();

                if (!int.TryParse(tokens[2], out int parsedBookId) || parsedBookId < 0)
                    throw new FormatException();

                if (!int.TryParse(tokens[3], out int parsedCount) || parsedCount < 0)
                    throw new FormatException();
            }
            catch
            {
                Console.WriteLine(errorMessage);
                Environment.Exit(0);
            }
        }

        private void ValidateBookFormat(string[] tokens)
        {
            try
            {
                if (!int.TryParse(tokens[1], out int bookId) || bookId < 0)
                    throw new FormatException();

                if (!int.TryParse(tokens[4], out int bookPrice) || bookPrice < 0)
                    throw new FormatException();

                if (ContainsInvalidChar(tokens[2]) || ContainsInvalidChar(tokens[3]))
                    throw new FormatException();
            }
            catch
            {
                Console.WriteLine("Data error.");
                Environment.Exit(0);
            }
        }

        public static ModelStore LoadFrom(TextReader reader)
        {
            var store = new ModelStore();

            try
            {
                if (reader.ReadLine() != "DATA-BEGIN")
                {
                    throw new FormatException("Data error.");
                }

                string line;
                while ((line = reader.ReadLine()) != null && line != "DATA-END")
                {
                    string[] tokens = line.Split(';');
                    switch (tokens[0])
                    {
                        case "BOOK":
                            store.ValidateBookFormat(tokens);
                            store.AddBook(tokens);
                            break;
                        case "CUSTOMER":
                            store.CheckCorrectFormatCustomer(tokens);
                            store.AddCustomer(tokens);
                            break;
                        case "CART-ITEM":
                            store.ValidateCartItemFormat(tokens);
                            store.AddCartItem(tokens);
                            break;
                        default:
                            throw new FormatException("Data error.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is IndexOutOfRangeException)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
                throw;
            }

            return store;
        }

        private void AddBook(string[] tokens)
        {
            books.Add(new Book
            {
                Id = int.Parse(tokens[1]),
                Title = tokens[2],
                Author = tokens[3],
                Price = decimal.Parse(tokens[4])
            });
        }

        private void AddCustomer(string[] tokens)
        {
            customers.Add(new Customer
            {
                Id = int.Parse(tokens[1]),
                FirstName = tokens[2],
                LastName = tokens[3]
            });
        }

        private void AddCartItem(string[] tokens)
        {
            var customer = GetCustomer(int.Parse(tokens[1]));
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found.");
            }

            customer.ShoppingCart.Items.Add(new ShoppingCartItem
            {
                BookId = int.Parse(tokens[2]),
                Count = int.Parse(tokens[3])
            });
        }

    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }

    public class Customer
    {
        private ShoppingCart shoppingCart;

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ShoppingCart ShoppingCart
        {
            get
            {
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart();
                }
                return shoppingCart;
            }
            set
            {
                shoppingCart = value;
            }
        }
    }

    public class ShoppingCartItem
    {
        public int BookId { get; set; }
        public int Count { get; set; }
    }

    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public List<ShoppingCartItem> Items = new List<ShoppingCartItem>();
    }


    



    //View
    
}