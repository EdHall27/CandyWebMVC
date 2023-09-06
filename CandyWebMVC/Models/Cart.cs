using System.ComponentModel.DataAnnotations;

namespace CandyWebMVC.Models
{
    public class Cart
    {
        [Required(ErrorMessage = "{0} required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public int IdProduct { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public Products? Product { get; set; } 

        [Required(ErrorMessage = "{0} required")]
        public int UserId { get; set; } // If tracking carts per user

        [Required(ErrorMessage = "{0} required")]
        public DateTime CreatedAt { get; set; }
    }
}
