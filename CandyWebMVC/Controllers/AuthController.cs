using Microsoft.AspNetCore.Mvc;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using CandyWebMVC.Models.ViewModel;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CandyWebMVC.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context; // DbContext

        public AuthController(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.CPFID == model.CPFID);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var isAdmin = Convert.ToBoolean(user.IsAdmin);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("CPFID", user.CPFID.ToString()),
                        new Claim("IsAdmin", user.IsAdmin.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "CPF ou senha inválidos.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login"); //Redireciona para a ação de login após logout
        }

        public IActionResult LoginRequired()
        {
            return View("LoginRequired");
        }
    }
}
