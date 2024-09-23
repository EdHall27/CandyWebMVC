namespace CandyWebMVC.Models.DTOs
{
    public class ProductDto
    {
        public int? Id { get; set; }  // Opcional para criação
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        public IFormFile? Image { get; set; }  // Arquivo de imagem
    }

}
