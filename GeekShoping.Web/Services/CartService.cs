using GeekShoping.Web.Services.IServices;
using GeekShopping.Web.Models;
using System.Net.Http.Headers;
using GeekShoping.Web.Utils;

namespace GeekShoping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private const string BasePath = "api/v1/Cart";

        public CartService(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<CartViewModel> FindByUserId(string userId, string token)
        {
            AddAuthentication(token);

            var response = await _client.GetAsync($"{BasePath}/find-cart?userId={userId}");

            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<CartViewModel> AddItemToCart(CartViewModel model, string token)
        {
            AddAuthentication(token);

            var response = await _client.PostAsJson($"{BasePath}/add-cart", model);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<CartViewModel> UpdateCart(CartViewModel model, string token)
        {
            AddAuthentication(token);

            var response = await _client.PutAsJson($"{BasePath}/update-cart", model);

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

        public async Task<bool> ApplyCoupon(CartViewModel cart, string token)
        {
            AddAuthentication(token);

            var response = await _client.PostAsJson($"{BasePath}/apply-coupon", cart);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<bool> RemoveCoupon(string userId, string token)
        {
            AddAuthentication(token);

            var response = await _client.DeleteAsync($"{BasePath}/remove-coupon?userId={userId}");

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        public async Task<bool> Clear(string userId, string token) 
        {
            throw new NotImplementedException();

        }

        public async Task<CartHeaderViewModel> Checkout(CartHeaderViewModel cartHeader, string token)
        {

            AddAuthentication(token);

            var response = await _client.PostAsJson($"{BasePath}/checkout", cartHeader);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartHeaderViewModel>();

            else throw new Exception($"Something went wrong calling the API. {response.StatusCode}");
        }

        private void AddAuthentication(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
