using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Repository.Interfaces;
using GeekShopping.ProductAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var products = await _repository.FindAll();

            return Ok(products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(long id)
        {
            var product = await _repository.FindById(id);

            return product != null
                ? Ok(product)
                : NotFound();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ProductDto dto)
        {
            if (dto == null) return BadRequest();

            var product = await _repository.Create(dto);

            return Ok(product);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody]ProductDto dto)
        {
            if (dto == null) return BadRequest();

            var product = await _repository.Update(dto);

            return Ok(product);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var product = await _repository.Delete(id);

            return product ? Ok(product) : BadRequest(); 
        }

    }
}
