using AutoMapper;
using GeekShopping.ProductAPI.Data.DTOs;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using GeekShopping.ProductAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _productMapper;
        private readonly SQLContext _context;

        public ProductRepository(IMapper mapper, SQLContext context)
        {
            _productMapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> FindAll()
        {
            var products =  await _context.Products.ToListAsync();

            return _productMapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> FindById(long id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            return _productMapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> Create(ProductDto dto)
        {
            var product = _productMapper.Map<Product>(dto);

            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return _productMapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> Update(ProductDto dto)
        {
            var product = _productMapper.Map<Product>(dto);

            _context.Update(product);

            await _context.SaveChangesAsync();

            var actualProduct = _productMapper.Map<ProductDto>(product);

            if (actualProduct != null)
                return _productMapper.Map<ProductDto>(product);
            else
                throw new Exception("Produto não atualizado.");
        }

        public async Task<bool> Delete(long id)
        {
            var product = _context.Products.Find(id);

            if(product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                return true;
            }
            else
                return false;
        }


    }
}
