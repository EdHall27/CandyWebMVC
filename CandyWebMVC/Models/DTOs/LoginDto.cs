namespace CandyWebMVC.Models.DTOs
{
    public class LoginDto
    {
        public int CPFID { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
