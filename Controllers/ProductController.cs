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
                try
                {
                var products = await _productService.GetAllProductsAsync();
                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "Товари не знайдено" });
                }
                var response = products.Select(p => p.ToProductDto());
                return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Помилка при отриманні товарів", error = ex.Message });
                }
            }
            [HttpGet("{id}")]
            public async Task<IActionResult> GetProductById(int id)
            {
                try
                {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product.ToProductDto());
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Помилка при отриманні товару", error = ex.Message });
                }
            }
            [HttpPost]
            public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
            {
                try
                {
                    if (productDto == null)
                    {
                        return BadRequest(new { message = "Невірні дані для створення товару" });
                    }
                    if (string.IsNullOrWhiteSpace(productDto.Name))
                    {
                        return BadRequest(new { message = "Назва товару не може бути порожньою" });
                    }
                    if (productDto.Price < 0)
                    {
                        return BadRequest(new { message = "Ціна товару не може бути від'ємною" });
                    }
                    if (productDto.Stock < 0)
                    {
                        return BadRequest(new { message = "Кількість товару на складі не може бути від'ємною" });
                    }
                    if(productDto.CategoryId <= 0)
                    {
                        return BadRequest(new { message = "Невірний ID категорії" });
                    }
                   
                    var product = await _productService.AddProductAsync(productDto);
                    return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Помилка при створенні товару", error = ex.Message });
                }
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
            {
            try
            {
                if (productDto == null)
                {
                    return BadRequest(new { message = "Невірні дані для оновлення товару" });
                }
                if (string.IsNullOrWhiteSpace(productDto.Name))
                {
                    return BadRequest(new { message = "Назва товару не може бути порожньою" });
                }
                if (productDto.Price < 0)
                {
                    return BadRequest(new { message = "Ціна товару не може бути від'ємною" });
                }
                if (productDto.Stock < 0)
                {
                    return BadRequest(new { message = "Кількість товару на складі не може бути від'ємною" });
                }
                var product = await _productService.UpdateProductAsync(id, productDto);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Помилка при оновленні товару", error = ex.Message });
                }
                
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProduct(int id)
            {
                try
                {
                    var result = await _productService.DeleteProductAsync(id);
                    if (!result)
                    {
                        return NotFound( new { message = "Товар не знайдено для видалення" });
                    }
                    return Ok(new { message = "Товар успішно видалено" });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Помилка при видаленні товару", error = ex.Message });
                }       
                
            }
            //SEARCH
                // GET: api/products/filter
            [HttpGet("filter")]
            public async Task<IActionResult> FilterProducts(
                [FromQuery] int? categoryId,
                [FromQuery] decimal? minPrice,
                [FromQuery] decimal? maxPrice,
                [FromQuery] string? search,
                [FromQuery] bool? inStock,
                [FromQuery] string? similar)
            {
                try
                {
                    var products = await _productService.GetAllProductsAsync();
                    var filtered = products.AsEnumerable();

                    // Фільтр за категорією
                    if (categoryId.HasValue)
                    {
                        filtered = filtered.Where(p => p.CategoryId == categoryId.Value);
                    }

                    // Фільтр за ціною
                    if (minPrice.HasValue)
                    {
                        filtered = filtered.Where(p => p.Price >= minPrice.Value);
                    }

                    if (maxPrice.HasValue)
                    {
                        filtered = filtered.Where(p => p.Price <= maxPrice.Value);
                    }

                    // Фільтр наявності
                    if (inStock.HasValue && inStock.Value)
                    {
                        filtered = filtered.Where(p => p.Stock > 0);
                    }

                    // Пошук за назвою або описом
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        filtered = filtered.Where(p =>
                            p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                            (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                        );
                    }

                    
                    if (!string.IsNullOrWhiteSpace(similar))
                    {
                        string request = similar.Trim().ToLower();
                        var keywords = request.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                      
                        var matched = filtered.Where(p =>
                            keywords.All(keyword => p.Name.ToLower().Contains(keyword))
                        ).ToList();

                        if (!matched.Any())
                        {
                            return NotFound(new { message = "Товари з такими ключовими словами не знайдено" });
                        }

                      
                        var groups = matched
                            .GroupBy(p => string.Join(" ", keywords))
                            .Select(g => new
                            {
                                Keyword = g.Key,
                                Count = g.Count(),
                                Items = g.Select(p => new
                                {
                                    p.Id,
                                    p.Name,
                                    p.Price,
                                    p.Stock,
                                    CategoryName = p.Category?.Name ?? "Без категорії"
                                }).OrderBy(p => p.Price) 
                            })
                            .ToList();

                        return Ok(groups);
                    }

                    
                    var result = filtered.Select(p => p.ToProductDto()).ToList();

                    if (!result.Any())
                    {
                        return NotFound(new { message = "Товари за заданими критеріями не знайдено" });
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }  
        }
    }
