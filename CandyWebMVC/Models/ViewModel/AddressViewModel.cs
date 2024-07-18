using Microsoft.AspNetCore.Mvc.Rendering;

namespace CandyWebMVC.Models.ViewModel
{
    public class AddressViewModel
    {
        public IEnumerable<Address> Addresses { get; set; } = new List<Address>();
        public IEnumerable<SelectListItem> AddressSelectList { get; set; } = new List<SelectListItem>(); // Usado para dropdown
        public Address NewAddress { get; set; } = new Address(); // Para adicionar um novo endereço
        public int? SelectedAddressId { get; set; } // Opcional: para selecionar um endereço existente
        public int CPFID { get; set; }
    }
}
