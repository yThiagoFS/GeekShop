using GeekShoping.Web.Models;
using GeekShoping.Web.Services.IServices;
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

        public HomeController(ILogger<HomeController> logger
            ,IProductService productService)
        {
            _logger = logger;
            _productService = productService;
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