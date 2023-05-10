using GeekShoping.Web.Services.IServices;
using GeekShopping.Web.Models;
using System.Net.Http.Headers;
using GeekShoping.Web.Utils;

namespace GeekShoping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private const string BasePath = "api/v1/cart";

        public CartService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CartViewModel> FindByUserId(string userId, string token)
        {
            AddAuthentication(token);

            var response = await _client.GetAsync($"{BasePath}/find-cart/{userId}");

            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
        {
            AddAuthentication(token);

            var response = await _client.PostAsJson<CartViewModel>($"{BasePath}/add-cart", model);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<CartViewModel> UpdateCart(CartViewModel model, string token)
        {
            AddAuthentication(token);

            var response = await _client.PutAsJson<CartViewModel>($"{BasePath}/update-cart", model);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<bool> RemoveFromCart(long cartId, string token)
        {
            AddAuthentication(token);

            var response = await _client.DeleteAsync($"{BasePath}/delete-cart/{cartId}");

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<bool> ApplyCoupon(CartViewModel cart, string couponCode, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Clear(string userId, string token) 
        {
            throw new NotImplementedException();

        }

        public async Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader, string token)
        {
            throw new NotImplementedException();
        }

        private void AddAuthentication(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
