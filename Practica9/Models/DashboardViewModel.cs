namespace Practica9.Models
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }

        public int TotalCustomers { get; set; }

        public List<Product> TopProducts { get; set; }

        public List<Category> Categories { get; set; }

        public int? SelectedCategoryID { get; set; }
    }
}
