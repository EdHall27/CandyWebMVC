using CandyWebMVC.Models;
using Newtonsoft.Json;
using System.Text;
using CandyWebMVC.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CandyWebMVC.Helper
{
    public class CartHelper
    {
        private readonly Context _context;

        public CartHelper(Context context)
        {
            _context = context;
        }

        public List<Cart> GetCartItems()
        {
            return _context.CartItems.Include(item => item.Product).ToList();
        }

        public void AddToCart(int productId, int quantity)
        {
            var existingCartItem = _context.CartItems.FirstOrDefault(item => item.IdProduct == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _context.SaveChanges();
            }
            else
            {
                var product = _context.Products.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    var cartItem = new Cart
                    {
                        IdProduct = product.Id,
                        ProductName = product.Name,
                        ProductPrice = (decimal)product.Price,
                        Product = product,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                    _context.SaveChanges();
                }
            }
            
        }

        public void RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(item => item.Id == cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }
    }
}

