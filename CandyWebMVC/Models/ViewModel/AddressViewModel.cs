using Microsoft.AspNetCore.Mvc.Rendering;

namespace CandyWebMVC.Models.ViewModel
{
    public class AddressViewModel
    {
        public int? SelectedAddressId { get; set; }
        public Address? NewAddress { get; set; }
        public IEnumerable<SelectListItem>? Addresses { get; set; }
        public int CPFID { get; set; } // Adicionado para garantir que possamos predefinir o CPFID se necessário
    }
}
