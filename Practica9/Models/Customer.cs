namespace Practica9.Models
{
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Country { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
