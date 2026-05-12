using System;
using System.Threading.Tasks;
using api.DTO.Product;
using api.DTO;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using api.Services.Interfaces;
using api.DTO.Category;

namespace api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductInterface _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductInterface productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                var product = await _productService.AddProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpGet("{id:int}")] 
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound(new { message = $"Товар з ID {id} не знайдено" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);

                if (product == null)
                {
                    return NotFound(new { message = $"Товар з ID {id} не знайдено" });
                }

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Товар успішно видалено" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }
    

            //SEARCH
                // GET: api/products/filter
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] List<int>? categoryIds,
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

                // Фільтр за категорією (враховує дочірні)
                if (categoryIds != null && categoryIds.Any())
                {
                    var allCategories = await _categoryService.GetAllCategories();
                    var flat = FlattenCategories(allCategories);

                    // Розгортаємо кожен обраний ID з його дочірніми
                    // і групуємо по type
                    var groupedByType = new Dictionary<string, HashSet<int>>();

                    foreach (var catId in categoryIds)
                    {
                        // Знаходимо тип категорії
                        var cat = flat.FirstOrDefault(c => c.Id == catId);
                        if (cat == null) continue;

                        var type = cat.Type;

                        if (!groupedByType.ContainsKey(type))
                            groupedByType[type] = new HashSet<int>();

                        // Додаємо сам ID і всі дочірні
                        groupedByType[type].Add(catId);
                        foreach (var childId in GetAllChildIds(flat, catId))
                            groupedByType[type].Add(childId);
                    }

                    // AND між групами, OR всередині групи
                    filtered = filtered.Where(p =>
                    {
                        var productCategoryIds = p.Categories.Select(c => c.Id).ToHashSet();

                        return groupedByType.All(group =>
                            group.Value.Any(id => productCategoryIds.Contains(id))
                        );
                    });
                }
                // Фільтр за ціною
                if (minPrice.HasValue)
                    filtered = filtered.Where(p => p.Price >= minPrice.Value);

                if (maxPrice.HasValue)
                    filtered = filtered.Where(p => p.Price <= maxPrice.Value);

                // Фільтр наявності
                if (inStock.HasValue && inStock.Value)
                    filtered = filtered.Where(p => p.Stock > 0);

                // Пошук за назвою або описом
                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(p =>
                        p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Similar — пошук за ключовими словами
                if (!string.IsNullOrWhiteSpace(similar))
                {
                    string request = similar.Trim().ToLower();
                    var keywords = request.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var matched = filtered.Where(p =>
                        keywords.All(keyword => p.Name.ToLower().Contains(keyword))
                    ).ToList();

                    if (!matched.Any())
                        return NotFound(new { message = "Товари з такими ключовими словами не знайдено" });

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
                                CategoryName = p.Categories.FirstOrDefault()?.Name ?? "Без категорії"                            
                                }).OrderBy(p => p.Price)
                        })
                        .ToList();

                    return Ok(groups);
                }

                var result = filtered.ToList();

                if (!result.Any())
                    return NotFound(new { message = "Товари за заданими критеріями не знайдено" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private HashSet<int> GetAllChildIds(List<CategoryDtoResponse> all, int parentId)
        {
            var result = new HashSet<int>();

            // Збираємо всі категорії в плаский список
            var flat = FlattenCategories(all);

            // Знаходимо всіх нащадків рекурсивно
            var children = flat.Where(c => c.ParentId == parentId);

            foreach (var child in children)
            {
                result.Add(child.Id);
                foreach (var id in GetAllChildIds(flat, child.Id))
                    result.Add(id);
            }

            return result;
        }

        // Допоміжний метод — розгортає дерево категорій у плаский список
        private List<CategoryDtoResponse> FlattenCategories(List<CategoryDtoResponse> categories)
        {
            var result = new List<CategoryDtoResponse>();

            foreach (var cat in categories)
            {
                result.Add(cat);
                if (cat.Children != null && cat.Children.Any())
                    result.AddRange(FlattenCategories(cat.Children));
            }

            return result;
        } 
    }
}
