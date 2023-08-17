using System.ComponentModel.DataAnnotations;

namespace CandyWebMVC.Models.ViewModel
{
    public class ImageViewModel
    {
        [Required(ErrorMessage = "{0} required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public double Price { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} required")]
        public IFormFile ImageFile { get; set; }
    }
}
