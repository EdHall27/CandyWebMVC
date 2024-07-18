using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public string CEP { get; set; } = string.Empty;

        // Foreign Key
        public int CPFID { get; set; }

        // Navigation property for EF Core
        public User? User { get; set; }
    }
}
