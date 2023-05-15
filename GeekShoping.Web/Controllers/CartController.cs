using GeekShoping.Web.Services.IServices;
using GeekShopping.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShoping.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CartController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }
        
        public async Task<IActionResult> Remove(long id)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault().Value;

            var response = await _cartService.RemoveFromCart(id, await GetToken());

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private async Task<CartViewModel> FindUserCart()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            var response = await _cartService.FindByUserId(userId, await GetToken());

            if (response?.CartHeader != null)
            {
                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }
            }

            return response;
        }
        private async Task<string> GetToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }

    }
}
