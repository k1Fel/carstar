using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO;
using api.Mappers.ProductMappers;
using api.Services;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    
    [ApiController]
    [Route("api/products")]
        public class ProductsController : ControllerBase
        {
            private readonly IProductInterface _productService;

            public ProductsController(IProductInterface productService)
            {
                _productService = productService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAllProducts()
            {
                var products = await _productService.GetAllProductsAsync();
                var response = products.Select(p => p.ToProductDto());
                return Ok(response);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetProductById(int id)
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product.ToProductDto());
            }

            [HttpPost]
            public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
            {
                var product = await _productService.AddProductAsync(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
            {
                var product = await _productService.UpdateProductAsync(id, productDto);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProduct(int id)
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
        }
    }
