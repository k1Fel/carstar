using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.Repository.Intrefaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
         
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
            public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductCategories!)
                    .ThenInclude(pc => pc.Category)
                .ToListAsync();
        }
       public async Task<Product> AddProductAsync(Product product, List<int> categoryIds)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            foreach (var categoryId in categoryIds)
            {
                await _context.ProductCategories.AddAsync(new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = categoryId
                });
            }
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(product.Id) ?? product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await _context.Products.Where(p => p.Id == id).ExecuteDeleteAsync();
            return true;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.ProductCategories!)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

       public async Task<Product?> UpdateProductAsync(int id, Product product, List<int> categoryIds)
        {
            await _context.Products.Where(p => p.Id == id).ExecuteUpdateAsync(p => p
                .SetProperty(p => p.Name, product.Name)
                .SetProperty(p => p.ImageUrl, product.ImageUrl)
                .SetProperty(p => p.Description, product.Description)
                .SetProperty(p => p.Price, product.Price)
                .SetProperty(p => p.Stock, product.Stock));

            // Оновити категорії — видалити старі, додати нові
            var existing = await _context.ProductCategories
                .Where(pc => pc.ProductId == id)
                .ToListAsync();
            _context.ProductCategories.RemoveRange(existing);

            foreach (var categoryId in categoryIds)
            {
                await _context.ProductCategories.AddAsync(new ProductCategory
                {
                    ProductId = id,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();
            return await GetProductByIdAsync(id);
        }
    }
}