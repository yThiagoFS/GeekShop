using AutoMapper;
using GeekShopping.CouponAPI.Data.Dtos;
using GeekShopping.CouponAPI.Model;

namespace GeekShopping.CouponAPI.Mapper
{
    public class CouponMapper : Profile
    {
        public CouponMapper()
        {
            CreateMap<Coupon, CouponDto>()
                .ReverseMap();
        }
    }
}
