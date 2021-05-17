using Supermarket.API.Models;
using System.Collections.Generic;

namespace Supermarket.API.ResourceModel
{
    public class CategoryProduct
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<Product> Products { get; set; } = new HashSet<Product>();
    }
}
