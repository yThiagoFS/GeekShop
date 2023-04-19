using GeekShoping.Web.Models;
using GeekShoping.Web.Services.IServices;
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

        public async Task<IActionResult> ProductIndex()
        {
            var productList = await _productService.FindAllProducts();

            return View(productList);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(model);

                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductUpdate(long id)
        {
            var product = await _productService.FindProductById(id);

            if(product != null)
                return View(product);

            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> ProductUpdate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProduct(model);

                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDeleteConfirm(long id)
        {
            var product = await _productService.FindProductById(id);

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> ProductDelete(long id)
        {
            var response = await _productService.DeleteProductById(id);

            if (response)
                return RedirectToAction(nameof(ProductIndex));

            return BadRequest();
        }
    }
}
