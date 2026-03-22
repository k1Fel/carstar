using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.DTO.Product;
using api.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace api.Mappers.ProductMappers
{
    public static class ProductMapper
    {
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Unknown"
            };
        }

        public static Product ToProduct(this ProductDto productDto)
        {
            return new Product
            {
                Id = productDto.Id,
                ImageUrl = productDto.ImageUrl,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
            };
        }
        public static Product FromUpdateToProduct(this UpdateProductDto productDto, int id)
        {
            return new Product
            {
                Id = id,
                ImageUrl = productDto.ImageUrl,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
            };
        }
        public static Product FromCreateToProduct(this CreateProductDto productDto)
        {
            return new Product
            {
                Name = productDto.Name,
                ImageUrl = productDto.ImageUrl,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId
                
            };
        }
        public static ProductResponseDto ToProductResponse(this Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Unknown"
            };
        }
        public static ProductResponseDto ToProductResponseDto(this Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                ImageUrl = product.ImageUrl,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? "Unknown"
            };
        }   
    }
}