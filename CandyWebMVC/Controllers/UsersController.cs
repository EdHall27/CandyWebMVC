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
using CandyWebMVC.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CandyWebMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Context _context;

        public UsersController(Context context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (cpfClaim == null) return Unauthorized();

            int cpf = int.Parse(cpfClaim);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == cpf);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.CPFID,
                user.UserName,
                user.UserEmail,
                user.UserPhone,
                user.IsAdmin,
                EnderecoPadrao = user.DefaultAddress != null ? new
                {
                    user.DefaultAddress.Street,
                    user.DefaultAddress.City,
                    user.DefaultAddress.State,
                    user.DefaultAddress.CEP
                } : null
            });
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (cpfClaim == null) return Unauthorized();

            int cpf = int.Parse(cpfClaim);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == cpf);
            if (user == null) return NotFound();

            user.UserEmail = dto.UserEmail;
            user.UserPhone = dto.UserPhone;
            await _context.SaveChangesAsync();

            return Ok("Perfil atualizado com sucesso.");
        }

        [Authorize]
        [HttpPut("addresses/{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var cpfClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;
            if (cpfClaim == null) return Unauthorized();

            int cpf = int.Parse(cpfClaim);
            var user = await _context.Users.Include(u => u.UserAddresses).FirstOrDefaultAsync(u => u.CPFID == cpf);
            if (user == null) return NotFound();

            var address = user.UserAddresses?.FirstOrDefault(a => a.Id == id);
            if (address == null) return BadRequest("Endereço não encontrado");

            user.DefaultAddressId = id;
            await _context.SaveChangesAsync();

            return Ok("Endereço padrão atualizado.");
        }
    }
}
