using GeekShopping.CouponAPI.Data.Dtos;
using GeekShopping.CouponAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CouponController : ControllerBase
    {
        private ICouponRepository _repository;

        public CouponController(ICouponRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CouponDto>> GetCouponById([FromQuery] string couponCode)
        {
            var coupon = await _repository.GetCouponByCouponCode(couponCode);

            if (coupon == null) return NotFound();

            return Ok(coupon);
        }
    };

}