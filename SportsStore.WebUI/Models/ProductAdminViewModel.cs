using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    public class ProductAdminViewModel
    {
        public Product product { get; set; }
        public PriceInfo[] prices { get; set; }
    }
}