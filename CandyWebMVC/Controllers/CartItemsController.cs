using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CandyWebMVC.Helper;

namespace CandyWebMVC.Controllers
{
    //[Authorize]
    public class CartItemsController : Controller
    {
        private readonly Context _context;

        public CartItemsController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redireciona para a página customizada de "Login Required"
                return RedirectToAction("LoginRequired", "Auth");
            }
            // Obtendo o userId do claim do usuário logado
            var userId = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;

            // Converte userId para int antes de usá-lo
            int parsedUserId = int.Parse(userId ?? "0");

            var cart = await _context.Carts.Include(c => c.Items)
                                           .ThenInclude(i => i.Product)
                                           .FirstOrDefaultAsync(c => c.UserId == parsedUserId);

            if (cart == null || !cart.Items.Any())
            {
                // Se não houver carrinho, cria uma visão vazia
                return View(new List<CartItem>());
            }


            return View(cart.Items.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
                int parsedUserId = int.Parse(userId ?? "0");

                var cart = await _context.Carts.Include(c => c.Items)
                                               .FirstOrDefaultAsync(c => c.UserId == parsedUserId);

                if (cart == null)
                {
                    cart = new Cart { UserId = parsedUserId, CreatedAt = DateTime.Now };
                    _context.Carts.Add(cart);
                }

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    item = new CartItem { ProductId = productId, Quantity = quantity };
                    cart.Items.Add(item);
                }
                else
                {
                    item.Quantity += quantity;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception details here to help with debugging
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }

        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("Error");
        }
    }
}
