using AutoMapper;
using Supermarket.API.Models;
using Supermarket.API.ResourceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Dtos.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<CategoryProducts, CategoryProductsDto>()
                .ForMember(dest => dest.Products,
                    opt => opt.Ignore()).ReverseMap();
        }
    }
}
