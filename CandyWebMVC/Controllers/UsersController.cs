using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using System.Security.Cryptography;
using System.Text;
using CandyWebMVC.Models.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CandyWebMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly Context _context;

        public UsersController(Context context)
        {
            _context = context;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var userCpfId = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;

            if (userCpfId == null)
            {
                return Redirect("Auth/Login"); // Redireciona para login se o CPFID não estiver disponível
            }
            // Mudança aqui para buscar apenas o usuário logado
            int cpfId = int.Parse(userCpfId);
            var user = await _context.Users
                                     .Where(u => u.CPFID == cpfId)
                                     .ToListAsync();

            return View(user);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.CPFID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var viewModel = new UserRegistrationViewModel
            {
                User = new User(), // Inicializa um novo User
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegistrationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                // ... (Verificação de CPF existente) ...
                viewModel.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);

                _context.Users.Add(viewModel.User);
                await _context.SaveChangesAsync();


                var loginModel = new LoginModel
                {
                    CPFID = viewModel.User.CPFID,
                    PasswordHash = viewModel.User.PasswordHash, // Use o hash já calculado
                    User = viewModel.User,
                };
                _context.loginModels.Add(loginModel);
                await _context.SaveChangesAsync();

                // Autenticar o usuário aqui
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, viewModel.User.UserName),
                    new Claim("CPFID", viewModel.User.CPFID.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var editModel = new EditUserViewModel // Suponha que você crie um ViewModel que não inclua a senha
            {
                CPFID = user.CPFID,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPhone = user.UserPhone,
                IsAdmin = user.IsAdmin
            };

            return View(editModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel editModel)
        {
            if (id != editModel.CPFID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = editModel.UserName;
                user.UserEmail = editModel.UserEmail;
                user.UserPhone = editModel.UserPhone;
                user.IsAdmin = editModel.IsAdmin;

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.CPFID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.CPFID == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.User'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.CPFID == id)).GetValueOrDefault();
        }
    }
}
