using GeekShopping.CartAPI.Data.Dtos;

namespace GeekShopping.CartAPI.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<CartDto> FindCartByUserId(string userId);

        Task<CartDto> SaveOrUpdateCart(CartDto cart);

        Task<bool> RemoveFromCart(long cartDetailsId);

        Task<bool> ApplyCoupon(string userId, string couponCode);

        Task<bool> RemoveCoupon(string userId);

        Task<bool> ClearCart(string userId);
    }
}
