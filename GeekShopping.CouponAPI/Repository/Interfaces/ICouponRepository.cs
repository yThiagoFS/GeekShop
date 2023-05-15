using GeekShopping.CouponAPI.Data.Dtos;

namespace GeekShopping.CouponAPI.Repository.Interfaces
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCouponCode(string couponCode);
    }
}
