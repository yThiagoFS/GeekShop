using AutoMapper;
using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Model;

namespace GeekShopping.ProductAPI.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<ProductDto, Product>()
                .ReverseMap();
        }
    }
}
