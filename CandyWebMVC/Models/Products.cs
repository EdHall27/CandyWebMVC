using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    [Table("Products")]
    public class Products
    {
        [Required(ErrorMessage = "{0} required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public double Price { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public string ImagePath { get; set; } = string.Empty;

    }
}
