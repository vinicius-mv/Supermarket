using AutoMapper;
using Supermarket.API.Dtos;
using Supermarket.API.Models;
using System.Collections.Generic;

namespace Supermarket.API.ResourceModels
{
    public class CategoryProducts
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<Product> Products { get; set; } = new HashSet<Product>();


        public CategoryProductsDto ConvertToDto(IMapper mapper)
        {
            var dto = mapper.Map<CategoryProductsDto>(this);
            dto.Products = mapper.Map<IEnumerable<ProductDto>>(this.Products);

            return dto;
        }
    }
}
