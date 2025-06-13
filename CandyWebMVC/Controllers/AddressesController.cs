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
using CandyWebMVC.Models.DTOs;


namespace CandyWebMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly Context _context;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(Context context, ILogger<AddressesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressResponseDto>>> GetAddresses()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetAddresses: Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            var addresses = await _context.Addresses
                .Where(a => a.CPFID == userId)
                .OrderByDescending(a => a.IsDefault) 
                .ThenBy(a => a.CreatedAt) 
                .ToListAsync();

            
            var addressDtos = addresses.Select(a => new AddressResponseDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                State = a.State,
                CEP = a.CEP, 
                IsDefault = a.IsDefault,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            return Ok(addressDtos);
        }

        // GET: api/addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressResponseDto>> GetAddress(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized("User ID not found.");

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.CPFID == userId);

            if (address == null)
                return NotFound($"Address with ID {id} not found for this user.");

            // Map Address entity to AddressResponseDto
            var addressDto = new AddressResponseDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                CEP = address.CEP,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            return Ok(addressDto);
        }

        // GET: api/addresses/default
        [HttpGet("default")]
        public async Task<ActionResult<AddressResponseDto>> GetDefaultAddress()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("GetDefaultAddress: Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            var defaultAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.CPFID == userId && a.IsDefault);

            if (defaultAddress == null)
            {
                _logger.LogInformation($"GetDefaultAddress: Nenhum endereço padrão encontrado para o utilizador {userId}.");
                return NotFound("Nenhum endereço padrão encontrado para este utilizador.");
            }

            // Map Address entity to AddressResponseDto
            var addressDto = new AddressResponseDto
            {
                Id = defaultAddress.Id,
                Street = defaultAddress.Street,
                City = defaultAddress.City,
                State = defaultAddress.State,
                CEP = defaultAddress.CEP,
                IsDefault = defaultAddress.IsDefault,
                CreatedAt = defaultAddress.CreatedAt,
                UpdatedAt = defaultAddress.UpdatedAt
            };

            return Ok(addressDto);
        }

        // POST: api/addresses
        [HttpPost]
        public async Task<ActionResult<AddressResponseDto>> PostAddress(AddressCreateUpdateDto addressDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning("PostAddress: Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("PostAddress: Erro de validação do modelo para o endereço.");
                return BadRequest(ModelState);
            }

            var address = new Address
            {
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                CEP = addressDto.CEP,
                CPFID = userId.Value,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDefault = addressDto.IsDefault // Use IsDefault from DTO if provided
            };


            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == userId.Value);
            if (user == null)
            {
                return NotFound("Utilizador associado ao CPFID não encontrado.");
            }

            var hasAddresses = await _context.Addresses.AnyAsync(a => a.CPFID == userId);

            if (!hasAddresses || address.IsDefault)
            {
                await RemoveDefaultFromOtherAddresses(userId.Value); 
                address.IsDefault = true; 
                user.DefaultAddressId = address.Id; 
            }

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            // Map the saved Address entity back to AddressResponseDto for the response
            var createdAddressDto = new AddressResponseDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                CEP = address.CEP,
                IsDefault = address.IsDefault,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, createdAddressDto);
        }

        // PUT: api/addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressCreateUpdateDto addressDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning($"PutAddress({id}): Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.CPFID == userId);

            if (existingAddress == null)
            {
                _logger.LogInformation($"PutAddress({id}): Endereço não encontrado para o utilizador {userId}.");
                return NotFound($"Endereço com ID {id} não encontrado para este utilizador.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"PutAddress({id}): Erro de validação do modelo para o endereço.");
                return BadRequest(ModelState);
            }

            // Tenta obter o utilizador para atualizar DefaultAddressId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == userId.Value);
            if (user == null)
            {
                return NotFound("Utilizador associado ao CPFID não encontrado.");
            }

            existingAddress.Street = addressDto.Street;
            existingAddress.City = addressDto.City;
            existingAddress.State = addressDto.State;
            existingAddress.CEP = addressDto.CEP; // Usa ZipCode no modelo Address
            existingAddress.UpdatedAt = DateTime.Now;

            // Lógica para definir/remover o padrão
            if (addressDto.IsDefault && !existingAddress.IsDefault)
            {
                // Se o DTO diz para ser padrão, e não era, define como padrão
                await RemoveDefaultFromOtherAddresses(userId.Value);
                existingAddress.IsDefault = true;
                user.DefaultAddressId = existingAddress.Id; // ATUALIZA O DEFAULTADDRESSID DO UTILIZADOR
            }
            else if (!addressDto.IsDefault && existingAddress.IsDefault)
            {
                // Se o DTO diz para NÃO ser padrão, e era, desmarca.
                // Mas apenas se houver mais de um endereço, caso contrário, deve permanecer padrão
                var addressCount = await _context.Addresses.CountAsync(a => a.CPFID == userId);
                if (addressCount > 1)
                {
                    existingAddress.IsDefault = false;
                    // Se o endereço desmarcado era o DefaultAddressId do utilizador, desmarcar no utilizador
                    if (user.DefaultAddressId == existingAddress.Id)
                    {
                        user.DefaultAddressId = null; // ATUALIZA O DEFAULTADDRESSID DO UTILIZADOR PARA NULO
                    }
                }
                else
                {
                    // Se for o único endereço, deve permanecer padrão
                    existingAddress.IsDefault = true; // Reverte, mantém como padrão
                    ModelState.AddModelError(nameof(addressDto.IsDefault), "Não é possível desmarcar como padrão o único endereço.");
                    return BadRequest(ModelState);
                }
            }


            try
            {
                await _context.SaveChangesAsync(); // Salva as alterações no endereço e no utilizador
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id, userId.Value))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // PUT: api/addresses/5/set-default
        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetAsDefault(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning($"SetAsDefault({id}): Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.CPFID == userId);

            if (address == null)
            {
                _logger.LogInformation($"SetAsDefault({id}): Endereço não encontrado para o utilizador {userId}.");
                return NotFound($"Endereço com ID {id} não encontrado para este utilizador.");
            }

            if (address.IsDefault)
            {
                return Ok(new { message = "Endereço já é o padrão." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == userId.Value);
            if (user == null)
            {
                return NotFound("Utilizador associado ao CPFID não encontrado.");
            }

            await RemoveDefaultFromOtherAddresses(userId.Value); 

            address.IsDefault = true;
            address.UpdatedAt = DateTime.Now;
            user.DefaultAddressId = address.Id; 

            await _context.SaveChangesAsync(); 

            return Ok(new { message = "Endereço definido como padrão com sucesso." });
        }

        // DELETE: api/addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning($"DeleteAddress({id}): Utilizador não autorizado ou ID de utilizador não encontrado/inválido.");
                return Unauthorized("Utilizador não autenticado ou ID de utilizador inválido.");
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.CPFID == userId);

            if (address == null)
            {
                _logger.LogInformation($"DeleteAddress({id}): Endereço não encontrado para o utilizador {userId}.");
                return NotFound($"Endereço com ID {id} não encontrado para este utilizador.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.CPFID == userId.Value);
            if (user == null)
            {
                return NotFound("Utilizador associado ao CPFID não encontrado.");
            }

            if (user.DefaultAddressId == address.Id)
            {
                user.DefaultAddressId = null; 

                var otherAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.CPFID == userId && a.Id != id);

                if (otherAddress != null)
                {
                    otherAddress.IsDefault = true;
                    otherAddress.UpdatedAt = DateTime.Now;
                    user.DefaultAddressId = otherAddress.Id; 
                }
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync(); 

            _logger.LogInformation($"Endereço com ID {id} eliminado com sucesso para o utilizador {userId}.");
            return NoContent();
        }

        // Métodos auxiliares
        private int? GetCurrentUserId()
        {
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

        private async Task RemoveDefaultFromOtherAddresses(int userId)
        {
            var defaultAddresses = await _context.Addresses
                .Where(a => a.CPFID == userId && a.IsDefault)
                .ToListAsync();

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
                addr.UpdatedAt = DateTime.Now;
            }
        }

        private bool AddressExists(int id, int userId)
        {
            return _context.Addresses.Any(a => a.Id == id && a.CPFID == userId);
        }
    }
}
