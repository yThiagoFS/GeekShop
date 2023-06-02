﻿using GeekShopping.Web.Models;

namespace GeekShoping.Web.Services.IServices
{
    public interface ICartService
    {
        Task<CartViewModel> FindByUserId(string userId, string token);

        Task<CartViewModel> AddItemToCart(CartViewModel cart, string token); 

        Task<CartViewModel> UpdateCart(CartViewModel cart, string token);

        Task<bool> RemoveFromCart(long cartId, string token);

        Task<bool> ApplyCoupon(CartViewModel cart, string token);

        Task<bool> RemoveCoupon(string userId, string token);

        Task<bool> Clear(string userId, string token);

        Task<CartHeaderViewModel> Checkout(CartHeaderViewModel cartHeader, string token);
    }
}
