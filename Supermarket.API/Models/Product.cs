using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public double UnitsInStock { get; set; }
        public DateTime RecordDate { get; set; }

        // ef nav
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
