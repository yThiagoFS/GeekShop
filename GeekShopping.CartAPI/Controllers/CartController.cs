using GeekShopping.CartAPI.Data.Dtos;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository ?? throw new ArgumentException(nameof(cartRepository));
        }

        [HttpGet("find-cart/{id}")]
        public async Task<ActionResult<CartDto>> FindById(string userId)
        {
            var cart = await _cartRepository.FindCartByUserId(userId);

            if (cart == null) return NotFound();

            return Ok(cart);
        } 
        
        [HttpPost("add-cart/{id}")]
        public async Task<ActionResult<CartDto>> AddCart(CartDto cartDto)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDto);

            if (cart == null) return NotFound();

            return Ok(cart);
        }
        
        [HttpPost("update-cart/{id}")]
        public async Task<ActionResult<CartDto>> UpdateCart(CartDto cartDto)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDto);

            if (cart == null) return NotFound();

            return Ok(cart);
        }
        
        [HttpPost("update-cart/{id}")]
        public async Task<ActionResult<CartDto>> RemoveCart(int id)
        {
            var status = await _cartRepository.RemoveFromCart(id);

            if (!status) return BadRequest();

            return Ok(status);
        }

    }
}
