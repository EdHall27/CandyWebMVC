using System.ComponentModel.DataAnnotations;

namespace CandyWebMVC.Models.DTOs
{
    public class AddressCreateUpdateDto
    {
        [Required(ErrorMessage = "{0} required")]
        [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        [StringLength(50, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} required")]
        [StringLength(10, ErrorMessage = "{0} cannot exceed {1} characters.")] // Assuming typical CEP/Zip Code length
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP format is invalid (e.g., XXXXX-XXX).")] // Basic CEP format validation
        public string CEP { get; set; } = string.Empty;

        // Allows the client to suggest if this should be the default address, but backend logic will confirm
        public bool IsDefault { get; set; } = false;
    }
}
