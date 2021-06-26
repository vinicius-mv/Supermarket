using AutoMapper;
using Supermarket.API.Models;
using Supermarket.API.ResourceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.V1.Dtos
{
    public class CategoryProductsDto
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        public CategoryProducts ConvertToModel(IMapper mapper)
        {
            var model = mapper.Map<CategoryProducts>(this);
            model.Products = mapper.Map<IEnumerable<Product>>(this.Products);

            return model;
        }
    }
}
