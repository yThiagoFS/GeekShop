using GeekShoping.Web.Models;
using GeekShoping.Web.Services.IServices;
using GeekShopping.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShoping.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger
            ,IProductService productService
            ,ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var productList = await _productService.FindAllProducts("");

            return View(productList);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.FindProductById(id, await GetToken());

            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductViewModel model)
        {
            CartViewModel cart = new()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailViewModel cartDetail = new()
            {
                Count = model.Count,
                ProductId = model.Id,
                Product = await _productService.FindProductById(model.Id, await GetToken())
            };

            List<CartDetailViewModel> cartDetails = new();

            cartDetails.Add(cartDetail);

            var response = await _cartService.AddItemToCart(cart, await GetToken());

            if(response != null)
                return RedirectToAction(nameof(Index));

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var acessToken = await HttpContext.GetTokenAsync("access_token");

            return RedirectToAction(nameof(Index));
        }  

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        private async Task<string> GetToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}