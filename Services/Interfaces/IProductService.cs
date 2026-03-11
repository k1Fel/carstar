using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Models;
namespace api.Services
{
    public interface IProductInterface
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(CreateProductDto productDto);
        Task<Product?> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}