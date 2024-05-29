using Microsoft.AspNetCore.Mvc;
using CandyWebMVC.Data;
using System.Linq;
using CandyWebMVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using CandyWebMVC.Models;

namespace CandyWebMVC.Controllers
{
    public class AddressesController : Controller
    {
        private readonly Context _context;

        public AddressesController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cpfIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Obtenha o CPF do usuário atual

            // Tenta converter a string CPF para int.
            if (int.TryParse(cpfIdString, out int cpfId))
            {
                var userAddresses = _context.Addresses.Where(a => a.UserID == cpfId).ToList();

                var model = new AddressViewModel
                {
                    Addresses = new SelectList(userAddresses, "AddressId", "FullAddress"), // 'FullAddress' é uma propriedade em 'Address' que você precisa definir
                    NewAddress = new Address()
                };
                return View(model);
            }
            else
            {
                // Se a conversão falhar, você deve decidir o que fazer. 
                // Por exemplo, retornar para uma página de erro ou uma view com mensagem de erro.
                // Aqui, estamos retornando para a página de erro padrão.
                return RedirectToAction("Error"); // Supondo que você tenha uma action "Error" definida para tratar erros
            }
        }

        public IActionResult Address(int cpfId)
        {
            var addresses = _context.Addresses.Where(a => a.UserID == cpfId).ToList();
            var model = new AddressViewModel
            {
                Addresses = new SelectList(addresses, "AddressId", "FullAddress"),
                NewAddress = new Address(),
                CPFID = cpfId // Define o CPFID para o novo endereço
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedAddressId.HasValue)
                {
                    // Lógica para atualizar o endereço
                }
                else
                {
                    _context.Addresses.Add(model.NewAddress);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index");
            }
            // Retorna à view de entrada para correções se o modelo não for válido
            return View("Create", model);
        }


        // Método GET para a criação de endereços
        public IActionResult Create()
        {
            var model = new AddressViewModel
            {
                // Assegure-se de que GetAvailableAddresses() nunca retorna null
                Addresses = GetAvailableAddresses(),
                NewAddress = new Address()  // Instancia um novo Address para ser preenchido no formulário
            };

            return View(model);
        }

        // Método POST para a criação de endereços
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Addresses.Add(model.NewAddress);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Redireciona para a lista de endereços
            }

            // Se não for válido, retorne para a view com o modelo atual
            model.Addresses = GetAvailableAddresses(); // Re-popula a lista para a view
            return View(model);
        }

        private IEnumerable<SelectListItem> GetAvailableAddresses()
        {
            // Este método deve retornar uma lista de SelectListItem. Se não houver endereços, ele deve retornar uma lista vazia, não null.
            return _context.Addresses.Select(a => new SelectListItem
            {
                Text = a.Street + ", " + a.City,
                Value = a.Id.ToString()
            }).ToList();
        }
    }
}
