using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CandyWebMVC.Models.ViewModel
{
    public class EditUserViewModel
    {
        public int CPFID { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? UserPhone { get; set; }

        public bool IsAdmin { get; set; }
    }
}
