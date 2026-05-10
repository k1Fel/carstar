using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.DTO.Product;
using api.Models;
namespace api.Services
{
    public interface IProductInterface
    {
        Task<List<ProductResponseDto?>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> AddProductAsync(CreateProductDto productDto);
        Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
    }
}