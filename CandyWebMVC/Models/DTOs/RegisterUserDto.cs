namespace CandyWebMVC.Models.DTOs
{
    public class RegisterUserDto
    {
        public int CPFID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
