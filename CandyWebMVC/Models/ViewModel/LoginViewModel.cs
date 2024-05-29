using System.ComponentModel.DataAnnotations;

namespace CandyWebMVC.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "CPF")]
        public int CPFID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
