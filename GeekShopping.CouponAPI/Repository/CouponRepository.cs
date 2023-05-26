﻿using AutoMapper;
using GeekShopping.CouponAPI.Data.Dtos;
using GeekShopping.CouponAPI.Model.Context;
using GeekShopping.CouponAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly SQLContext _context;
        private readonly IMapper _mapper;

        public CouponRepository(SQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CouponDto> GetCouponByCouponCode(string couponCode)
        {
            try
            {
                var coupon =  await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

                return _mapper.Map<CouponDto>(coupon);
            }
            catch(Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }

        }
    }
}
