using AutoMapper;
using Supermarket.API.Models;
using Supermarket.API.ResourceModels;
using Supermarket.API.V2.Dtos;

namespace Supermarket.API.V2.Mappings
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
