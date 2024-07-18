using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Products Product { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public int Quantity { get; set; }

        public int CartId { get; set; } // Reference to Cart

        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; } // Navigation property to Cart
    }
}
