namespace CandyWebMVC.Models.DTOs
{
    public class UserProfileResponseDto
    {
        public int Cpfid { get; set; } 
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }

        public AddressResponseDto? DefaultAddress { get; set; }
    }
}
