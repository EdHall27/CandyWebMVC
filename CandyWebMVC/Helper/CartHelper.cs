using CandyWebMVC.Models;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using System.Linq;
using System.Security.Claims;

namespace CandyWebMVC.Helper
{
    public class CartHelper
    {
        private readonly Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartHelper(Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Recuperar ID do usuário logado
        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue("CPFID"));
        }

        public List<CartItem> GetCartItems()
        {
            int userId = GetCurrentUserId();
            var cart = _context.Carts.Include(c => c.Items)
                                     .ThenInclude(i => i.Product)
                                     .FirstOrDefault(c => c.UserId == userId);

            return (List<CartItem>)(cart?.Items ?? new List<CartItem>());
        }

        public void AddToCart(int productId, int quantity)
        {
            int userId = GetCurrentUserId();
            var cart = _context.Carts.Include(c => c.Items)
                                     .FirstOrDefault(c => c.UserId == userId);

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                // Incrementa a quantidade do item existente
                cartItem.Quantity += quantity;
            }
            else
            {
                // Adiciona novo item ao carrinho
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Product = product
                    };
                    cart.Items.Add(cartItem);
                }
            }

            _context.SaveChanges();
        }

        public void RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

    }
}
