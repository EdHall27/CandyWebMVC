using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandyWebMVC.Data;
using CandyWebMVC.Models;
using CandyWebMVC.Models.ViewModel;
using CandyWebMVC.Helper;
using Microsoft.AspNetCore.Authorization;
using CandyWebMVC.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace CandyWebMVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly Context _context;
        private readonly ImageHelper _imageHelper;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(Context context, ILogger<ProductsController> logger)
        {
            _context = context;
            _imageHelper = new ImageHelper();
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        {
            if(_context.Products == null)
                return NotFound("No products found.");
            return Ok(await _context.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Products>> CreateProduct([FromForm] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Products
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                Stock = productDto.Stock
            };

            if (productDto.Image != null)
            {
                var validationResult = ValidateImage(productDto.Image);
                if (validationResult != null)
                {
                    return BadRequest(validationResult);
                }

                var imagePath = _imageHelper.SaveImageAndGetPath(productDto.Image);
                product.ImagePath = imagePath;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            _logger.LogInformation("Iniciando a atualização do produto com ID {ProductId}", id);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Retornar o estado do modelo para ver o que está errado
            }

            if (id != productDto.Id)
            {
                _logger.LogWarning("Product ID mismatch: esperado {ExpectedId}, recebido {ReceivedId}", id, productDto.Id);
                return BadRequest("Product ID mismatch.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Produto com ID {ProductId} não encontrado", id);
                return NotFound();
            }

            product.Name = productDto.Name;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.Stock = productDto.Stock;

            if (productDto.Image != null)
            {
                _logger.LogInformation("Atualizando a imagem do produto com ID {ProductId}", id);
                var validationResult = ValidateImage(productDto.Image);
                if (validationResult != null)
                {
                    _logger.LogError("Erro de validação da imagem: {ValidationError}", validationResult);
                    return BadRequest(validationResult);
                }

                // Excluir imagem antiga se existir
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    _logger.LogInformation("Excluindo a imagem antiga do produto com ID {ProductId}", id);
                    _imageHelper.DeleteImageFile(product.ImagePath);
                }

                var imagePath = _imageHelper.SaveImageAndGetPath(productDto.Image);
                product.ImagePath = imagePath;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Produto com ID {ProductId} atualizado com sucesso", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Erro de concorrência ao atualizar o produto com ID {ProductId}", id);
                if (!ProductsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private string ValidateImage(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return "Tipo de arquivo não permitido.";
            }

            if (image.Length > 5 * 1024 * 1024) // 5 MB
            {
                return "O tamanho do arquivo excede o limite permitido.";
            }

            return null;
        }
    }
}

