using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
namespace api.Repository.Intrefaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product, List<int> categoryIds);        
        Task<Product?> UpdateProductAsync(int id, Product product, List<int> categoryIds);
        Task<bool> DeleteProductAsync(int id);
    }
}