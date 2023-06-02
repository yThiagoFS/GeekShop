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
        private readonly ICouponService _couponService;

        public CartController(IProductService productService
               ,ICartService cartService
               ,ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindUserCart());
        }  
        
        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartViewModel model)
        {
            var userId = GetUserId();

            var response = await _cartService.ApplyCoupon(model, await GetToken());

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        } 
        
        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var userId = GetUserId();

            var response = await _cartService.RemoveCoupon(userId, await GetToken());

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }
        
        public async Task<IActionResult> Remove(long id)
        {
            var userId = GetUserId();

            var response = await _cartService.RemoveFromCart(id, await GetToken());

            if (response)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await FindUserCart());
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CartViewModel model)
        {
            var response = await _cartService.Checkout(model.CartHeader, await GetToken());

            if(response != null)
            {
                return RedirectToAction(nameof(Confirmation));
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Confirmation()
        {
            return View();
        }

        private async Task<CartViewModel> FindUserCart()
        {
            var userId = GetUserId();

            var response = await _cartService.FindByUserId(userId, await GetToken());

            if (response?.CartHeader != null)
            {
                if(!string.IsNullOrEmpty(response.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode, await GetToken());

                    if(coupon?.CouponCode != null)
                    {
                        response.CartHeader.DiscountTotal = coupon.DiscountAmount;
                    }
                }

                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }

                response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountTotal;
            }

            return response;
        }

        private async Task<string> GetToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }

        private string? GetUserId()
        {
            return  User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
        }

    }
}
