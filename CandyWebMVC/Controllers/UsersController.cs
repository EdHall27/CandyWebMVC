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
        private readonly ILogger<UsersController> _logger;

        public UsersController(Context context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetProfile: Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            // Encontrar o utilizador pelo ID e INCLUIR o DefaultAddress
            var user = await _context.Users
                .AsNoTracking() // Não é necessário rastrear para uma operação de leitura
                .Include(u => u.DefaultAddress) // NOVO: Carrega o endereço padrão diretamente
                .FirstOrDefaultAsync(u => u.CPFID == userId); // Use CPFID, pois é a chave primária no seu modelo User

            if (user == null)
            {
                _logger.LogInformation($"GetProfile: Utilizador com ID {userId} não encontrado.");
                return NotFound($"Utilizador com ID {userId} não encontrado.");
            }

            // O defaultAddress já está carregado na propriedade user.DefaultAddress
            var defaultAddressDto = user.DefaultAddress != null ? new AddressResponseDto
            {
                Id = user.DefaultAddress.Id,
                Street = user.DefaultAddress.Street,
                City = user.DefaultAddress.City,
                State = user.DefaultAddress.State,
                CEP = user.DefaultAddress.CEP, // Mapeia o ZipCode do modelo para o 'cep' do DTO (minúsculas)
                IsDefault = user.DefaultAddress.IsDefault,
                CreatedAt = user.DefaultAddress.CreatedAt,
                UpdatedAt = user.DefaultAddress.UpdatedAt
            } : null; // Se não houver endereço padrão, retorna null

            // --- DEBGUING LOGS NOVO ---
            if (defaultAddressDto != null)
            {
                _logger.LogInformation($"GetProfile: DefaultAddressDto criado com sucesso! CEP no DTO: {defaultAddressDto.CEP}");
            }
            else
            {
                _logger.LogWarning($"GetProfile: DefaultAddressDto é NULL antes de enviar para o frontend.");
            }
            // --- FIM DOS DEBUGGING LOGS ---

            // Mapear para o DTO de resposta do perfil
            var userProfileDto = new UserProfileResponseDto
            {
                Cpfid = user.CPFID, // Use user.CPFID para o DTO
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPhone = user.UserPhone,
                IsAdmin = user.IsAdmin,
                DefaultAddress = defaultAddressDto // Atribui o DTO do endereço padrão
            };

            return Ok(userProfileDto);
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

        private int? GetCurrentUserId()
        {
            // Procura a claim com o tipo "CPFID" que você configurou
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CPFID")?.Value;

            if (userIdClaim == null)
            {
                _logger.LogError("GetCurrentUserId: Claim 'CPFID' não encontrada no Principal do Utilizador.");
                return null;
            }

            if (int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogInformation($"GetCurrentUserId: ID do utilizador '{userId}' extraído com sucesso.");
                return userId;
            }
            else
            {
                _logger.LogError($"GetCurrentUserId: Não foi possível converter o valor da claim 'CPFID' ('{userIdClaim}') para um inteiro.");
                return null;
            }
        }
    }
}
