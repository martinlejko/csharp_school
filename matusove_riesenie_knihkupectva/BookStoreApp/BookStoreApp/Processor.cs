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
    public class Processor
    {
        private ModelStore store;

        public Processor(ModelStore _store)
        {
            store = _store;
        }

        public void UpdateShoppingCart(string AddRemove, int customerId, int bookId)
        {
            switch (AddRemove)
            {
                case "Add":
                    AddBookToCart(customerId, bookId);
                    break;
                case "Remove":
                    RemoveBookFromCart(customerId, bookId);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported action type.");
            }
        }

        private void AddBookToCart(int customerId, int bookId)
        {
            if (!store.ValidateBook(bookId))
            {
                throw new InvalidOperationException("Book not available.");
            }

            var customer = store.GetCustomer(customerId);
            var shoppingCart = customer.ShoppingCart;
            var cartItem = shoppingCart.Items.Find(item => item.BookId == bookId);

            if (cartItem == null)
            {
                shoppingCart.Items.Add(new ShoppingCartItem { 
                    BookId = bookId, 
                    Count = 1 });
            }
            else
            {
                cartItem.Count++;
            }
        }

        private void RemoveBookFromCart(int customerId, int bookId)
        {
            var customer = store.GetCustomer(customerId);
            var shoppingCart = customer.ShoppingCart;
            var cartItem = shoppingCart.Items.Find(item => item.BookId == bookId);

            if (cartItem == null)
            {
                throw new InvalidOperationException("Item not found in cart.");
            }

            if (cartItem.Count > 1)
            {
                cartItem.Count--;
            }
            else
            {
                shoppingCart.Items.Remove(cartItem);
            }
        }
    }
}
