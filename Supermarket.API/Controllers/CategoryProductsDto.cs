using Supermarket.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Controllers
{
    public class CategoryProductsDto
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<ProductDto> Products { get; set; } = new HashSet<ProductDto>();
    }
}
