using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Category;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.Mappers.CategoryMappers;
namespace api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            try
            {
                var category = await _categoryService.CreateCategory(createCategoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Помилка при створенні категорії", error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
            var category = await _categoryService.GetCategoryById(id);
            return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Помилка при отриманні категорії", error = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Помилка при отриманні категорій", error = ex.Message });
            }

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            try{
                var category = await _categoryService.UpdateCategory(id, updateCategoryDto);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Помилка при оновленні категорії", error = ex.Message }); 
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategory(id);
                if (!result)
                {
                    return NotFound(new { message = "Категорія не знайдена" });
                }
                return Ok(new { message = "Категорія успішно видалена" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Помилка при видаленні категорії", error = ex.Message });
            }
        }
    }
}