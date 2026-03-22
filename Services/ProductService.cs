using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.DTO.Product;
using api.Mappers;
using api.Mappers.ProductMappers;
using api.Repository.Intrefaces;

namespace api.Services
{
    public class ProductService : IProductInterface
    {
        private readonly IProductRepository _productRepository; 
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductResponseDto> AddProductAsync(CreateProductDto productDto)
        {
            var category = await _categoryRepository.GetCategoryById(productDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Категорія з ID {productDto.CategoryId} не знайдена");
            }

            var product = productDto.FromCreateToProduct();

            var addedProduct = await _productRepository.AddProductAsync(product);

            return addedProduct.ToProductResponseDto();
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                throw new ArgumentException($"Товар з ID {id} не знайдено");
            }

            return await _productRepository.DeleteProductAsync(id);
        }

        public async Task<List<ProductResponseDto?>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();

            return products.Select(p => p?.ToProductResponseDto()).ToList();
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            
            var product = await _productRepository.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return null; 
            }

            return product.ToProductResponseDto(); 
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new ArgumentException($"Товар з ID {id} не знайдено");
            }

            var category = await _categoryRepository.GetCategoryById(productDto.CategoryId);
            if (category == null)
            {
                throw new ArgumentException($"Категорія з ID {productDto.CategoryId} не знайдена");
            }

            var product = productDto.FromUpdateToProduct(id);

            var updatedProduct = await _productRepository.UpdateProductAsync(id, product);

            return updatedProduct?.ToProductResponseDto();
        }
    }
}