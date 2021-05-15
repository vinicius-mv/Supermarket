using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Models
{
    public class Category
    {
        public Category()
        {
            Products = new Collection<Product>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        // ef nav
        public ICollection<Product> Products { get; set; }
    }
}
