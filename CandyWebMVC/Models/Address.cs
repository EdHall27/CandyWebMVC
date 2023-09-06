namespace CandyWebMVC.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int Userid { get; set; }

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public int CEP { get; set; }

        public User? User { get; set; }

    }
}
