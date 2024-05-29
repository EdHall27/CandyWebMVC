using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CandyWebMVC.Models.ViewModel
{
    public class UserRegistrationViewModel
    {
        public User User { get; set; } = new User(); // Inicializa para evitar null

        [Required(ErrorMessage = "{0} required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}
