namespace CandyWebMVC.Models
{
    public class User
    {
        public int CPFID { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public int UserPhone { get; set; }

        public ICollection<Address>? UserAdress   { get; set; } 
    }
}
