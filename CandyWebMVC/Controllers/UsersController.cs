using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using CandyWebMVC.Models.ViewModel;
using System.Security.Claims;
using BCrypt.Net;


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
            // Acessar a tabela de usuários diretamente, sem precisar de DbSet<User> explícito
            var users = await _context.Users.ToListAsync();
            return View(users);
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
            

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors)
                               .Select(x => x.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error); // Ou você pode usar o logger
                }

                // Aqui você pode retornar a view com os erros para depuração
                //return View(viewModel);
            }

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
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CPFID,UserName,UserEmail,UserPhone")] User user)
        {
            if (id != user.CPFID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            return View(user);
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

        public async Task<IActionResult> AddAddress(Address address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assegura que o usuário está logado
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redireciona para login se não estiver logado
            }

            if (ModelState.IsValid)
            {
                address.UserID = int.Parse(userId);
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction("Profile"); // Redireciona para a página de perfil
            }

            return View(address);
        }

    }
}
