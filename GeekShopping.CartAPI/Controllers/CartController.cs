﻿using GeekShopping.CartAPI.Data.Dtos;
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

        [HttpGet("find-cart")]
        public async Task<ActionResult<CartDto>> FindById([FromQuery] string userId)
        {
            var cart = await _cartRepository.FindCartByUserId(userId);

            if (cart == null) return NotFound();

            return Ok(cart);
        } 
        
        [HttpPost("add-cart")]
        public async Task<ActionResult<CartDto>> AddCart(CartDto cartDto)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDto);

            if (cart == null) return NotFound();

            return Ok(cart);
        }
        
        [HttpPut("update-cart")]
        public async Task<ActionResult<CartDto>> UpdateCart(CartDto cartDto)
        {
            var cart = await _cartRepository.SaveOrUpdateCart(cartDto);

            if (cart == null) return NotFound();

            return Ok(cart);
        }
        
        [HttpDelete("delete-cart/{id}")]
        public async Task<ActionResult<CartDto>> RemoveCart(int id)
        {
            var status = await _cartRepository.RemoveFromCart(id);

            if (!status) return BadRequest();

            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<CartDto>> ApplyCoupon(CartDto dto)
        {
            var status = await _cartRepository.ApplyCoupon(dto.CartHeader.UserId, dto.CartHeader.CouponCode);

            if (!status) return NotFound();

            return Ok(status);
        }
        
        [HttpDelete("remove-coupon")]
        public async Task<ActionResult<CartDto>> RemoveCoupon([FromQuery]string userId)
        {
            var status = await _cartRepository.RemoveCoupon(userId);

            if (!status) return NotFound();

            return Ok(status);
        }

    }
}
