using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using CandyWebMVC.Helper;

namespace CandyWebMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly CartHelper _cartHelper;

        public CartController(CartHelper cartHelper)
        {
            _cartHelper = cartHelper;
        }

        public IActionResult Index()
        {
            var cartItems = _cartHelper.GetCartItems();
            // Calcular o subtotal
            var subtotal = cartItems.Sum(item => item.Quantity * item.ProductPrice);

            // Passar o subtotal para a view via ViewBag ou como parte de um modelo de view
            ViewBag.Subtotal = subtotal;

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            _cartHelper.AddToCart(productId, quantity);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int cartItemId)
        {
            _cartHelper.RemoveFromCart(cartItemId);

            return RedirectToAction(nameof(Index));
        }
    }

}
