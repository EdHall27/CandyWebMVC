namespace CandyWebMVC.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        // Foreign key to Customer
        public int UserId { get; set; }
        public User User { get; set; }

        // Foreign key to Address
        public int AddressId { get; set; }
        public Address Address { get; set; }

        // Collection of products
        public ICollection<Products> Products { get; set; }

    }
}
