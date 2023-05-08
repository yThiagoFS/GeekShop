using AutoMapper;
using GeekShopping.CartAPI.Data.Dtos;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<ProductDto, Product>()
                .ReverseMap();
            CreateMap<CartHeaderDto, CartHeader>()
                .ReverseMap();
            CreateMap<CartDetailDto, CartDetail>()
                .ReverseMap();
            CreateMap<CartDto, Cart>()
                .ReverseMap();
        }
    }
}
