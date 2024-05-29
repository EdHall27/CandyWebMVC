using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int CPFID { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? UserPhone { get; set; }

        
        public string? PasswordHash { get; set; } // Armazene apenas o hash da senha

        public virtual ICollection<Address>? UserAddresses { get; set; }
    }
}
