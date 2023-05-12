using AutoMapper;
using GeekShopping.CartAPI.Data.Dtos;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly SQLContext _context;
        private readonly IMapper _mapper;

        public CartRepository(SQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDto> FindCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId)
            };

            cart.CartDetails = _context.CartDetails
                .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> SaveOrUpdateCart(CartDto dto)
        {
            Cart cart = _mapper.Map<Cart>(dto);

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.CartDetails.FirstOrDefault().ProductId);

                if (product == null)
                {
                    await _context.Products.AddAsync(cart.CartDetails.FirstOrDefault().Product);

                    await _context.SaveChangesAsync();
                }

                var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);

                if (cartHeader == null)
                {
                    await _context.CartHeaders.AddAsync(cart.CartHeader);

                    await _context.SaveChangesAsync();

                    cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;

                    await _context.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());

                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cartDetail = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId && p.CartHeaderId == cartHeader.Id);

                    if (cartDetail == null)
                    {
                        cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                        cart.CartDetails.FirstOrDefault().Product = null;

                        await _context.CartDetails.AddAsync(cart.CartDetails.FirstOrDefault());

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cartDetail.Product = null;
                        cartDetail.Count += cart.CartDetails.FirstOrDefault().Count;

                        _context.CartDetails.Update(cartDetail);

                        await _context.SaveChangesAsync();
                    }
                }
            } 
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
           

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetailsId);

                int total = _context.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

                _context.CartDetails.Remove(cartDetail);

                if(total == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> ApplyCoupon(string userId, string coupon)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeader =  await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

            if(cartHeader != null)
            {
                _context.CartDetails.RemoveRange(
                    _context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));

                _context.CartHeaders.Remove(cartHeader);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
