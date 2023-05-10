using GeekShoping.Web.Models;
using GeekShoping.Web.Services.IServices;
using GeekShoping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShoping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> ProductIndex()
        {
            var productList = await _productService.FindAllProducts("");

            return View(productList);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(model, await GetToken());

                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductUpdate(long id)
        {
            var product = await _productService.FindProductById(id, await GetToken());

            if(product != null)
                return View(product);

            return NotFound();
        }


        [Authorize]
        [HttpPut]
        public async Task<IActionResult> ProductUpdate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(model, await GetToken());

                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ProductDeleteConfirm(long id)
        {
            var product = await _productService.FindProductById(id, await GetToken());

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(long id)
        {
            var response = await _productService.DeleteProductById(id, await GetToken());

            if (response)
                return RedirectToAction(nameof(ProductIndex));

            return BadRequest();
        }

        private async Task<string> GetToken()
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
    }
}
