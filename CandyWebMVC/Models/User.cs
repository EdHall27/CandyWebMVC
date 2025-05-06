using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int CPFID { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        [Phone]
        public string? UserPhone { get; set; }

        public bool IsAdmin { get; set; }

        public string? PasswordHash { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        public int? DefaultAddressId { get; set; }

        [ForeignKey("DefaultAddressId")]
        public Address? DefaultAddress { get; set; }

        public virtual ICollection<Address>? UserAddresses { get; set; }
    }
}
