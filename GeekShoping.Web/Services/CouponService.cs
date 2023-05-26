using GeekShoping.Web.Models;
using GeekShoping.Web.Services.IServices;
using GeekShoping.Web.Utils;
using System.Net.Http.Headers;

namespace GeekShoping.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly HttpClient _client;
        private const string BasePath = "/api/v1/Coupon";

        public CouponService(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponViewModel> GetCoupon(string code, string token)
        {
            AddAuthentication(token);

            var response = await _client.GetAsync($"{BasePath}?couponCode={code}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK) return new CouponViewModel();

            return await response.ReadContentAs<CouponViewModel>();

        }

        private void AddAuthentication(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
