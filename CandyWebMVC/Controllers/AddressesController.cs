using Microsoft.AspNetCore.Mvc;
using CandyWebMVC.Data;
using System.Linq;
using CandyWebMVC.Models;
using CandyWebMVC.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CandyWebMVC.Controllers
{
    public class AddressesController : Controller
    {
        private readonly Context _context;

        public AddressesController(Context context)
        {
            _context = context;
        }

        // Listar todos os endereços associados ao CPFID do usuário logado
        public async Task<IActionResult> Index()
        {
            var cpfIdString = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfIdString, out int cpfIdInt))
            {
                return View("Error", new ErrorViewModel { RequestId = "CPFID não disponível" });
            }

            var addresses = await _context.Addresses.Where(a => a.CPFID == cpfIdInt).ToListAsync();
            return View(addresses);
        }

        // Criar um novo endereço
        public IActionResult Create()
        {
            var model = new Address();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address model)
        {
            if (ModelState.IsValid)
            {
                // Pega o CPFID do usuário logado
                var cpfIdString = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
                if (int.TryParse(cpfIdString, out int cpfId))
                {
                    model.CPFID = cpfId; // Atribui o CPFID ao novo endereço
                    _context.Addresses.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao identificar usuário.");
                }
            }
            return View(model);
        }

        // Editar um endereço existente
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null || address.CPFID != int.Parse(User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value ?? "0"))
            {
                return NotFound();
            }

            var model = new AddressViewModel
            {
                NewAddress = address,
                CPFID = address.CPFID
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddressViewModel viewModel)
        {
            if (id != viewModel.NewAddress.Id || viewModel.CPFID != int.Parse(User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var address = await _context.Addresses.FindAsync(id);
                if (address == null)
                {
                    return NotFound();
                }

                // Atualiza propriedades
                address.Street = viewModel.NewAddress.Street;
                address.City = viewModel.NewAddress.City;
                address.State = viewModel.NewAddress.State;
                address.CEP = viewModel.NewAddress.CEP;
                

                _context.Update(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
