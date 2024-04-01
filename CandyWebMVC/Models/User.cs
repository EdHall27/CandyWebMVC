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
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        public int UserPhone { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public ICollection<Address>? UserAdress   { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Password { get; set; } = string.Empty;

        //[Required(ErrorMessage = "{0} required")]
        //public Login Login { get; set; }
    }
}
