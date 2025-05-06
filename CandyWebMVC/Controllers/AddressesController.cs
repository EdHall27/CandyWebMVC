using Microsoft.AspNetCore.Mvc;
using CandyWebMVC.Data;
using System.Linq;
using CandyWebMVC.Models;
using CandyWebMVC.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace CandyWebMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly Context _context;

        public AddressesController(Context context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfClaim, out int cpfId)) return Unauthorized();

            var addresses = await _context.Addresses
                .Where(a => a.CPFID == cpfId)
                .ToListAsync();

            return Ok(addresses);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] Address model)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfClaim, out int cpfId)) return Unauthorized();

            model.CPFID = cpfId;
            _context.Addresses.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Endereço adicionado com sucesso.");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditAddress(int id, [FromBody] Address model)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfClaim, out int cpfId)) return Unauthorized();

            var address = await _context.Addresses.FindAsync(id);
            if (address == null || address.CPFID != cpfId) return NotFound();

            address.Street = model.Street;
            address.City = model.City;
            address.State = model.State;
            address.CEP = model.CEP;

            await _context.SaveChangesAsync();

            return Ok("Endereço atualizado com sucesso.");
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfClaim, out int cpfId)) return Unauthorized();

            var address = await _context.Addresses.FindAsync(id);
            if (address == null || address.CPFID != cpfId) return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Ok("Endereço removido com sucesso.");
        }

        [Authorize]
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (!int.TryParse(cpfClaim, out int cpfId)) return Unauthorized();

            var user = await _context.Users.Include(u => u.UserAddresses).FirstOrDefaultAsync(u => u.CPFID == cpfId);
            if (user == null) return NotFound();

            var address = user.UserAddresses?.FirstOrDefault(a => a.Id == id);
            if (address == null) return BadRequest("Endereço não encontrado");

            user.DefaultAddressId = id;
            await _context.SaveChangesAsync();

            return Ok("Endereço padrão atualizado.");
        }
    }
}
