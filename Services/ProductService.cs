using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Mappers.ProductMappers;
using api.Models;
using api.Repository;
using api.Repository.Intrefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace api.Services
{
    public class ProductService : IProductInterface
    {
        private readonly IProductRepository _productRepository; 
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> AddProductAsync(CreateProductDto productDto)
        {
            var product = productDto.FromCreateToProduct();
            var addedProduct =  await _productRepository.AddProductAsync(product);
            return addedProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _productRepository.DeleteProductAsync(id);
            return result;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
           var products = await _productRepository.GetAllProductsAsync();
           return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var products =  await _productRepository.GetProductByIdAsync(id);
            return products;
        }

        public async Task<Product?> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = productDto.FromUpdateToProduct(id);
            var products = await _productRepository.UpdateProductAsync(id, product);
            return products;
        }
    }
}