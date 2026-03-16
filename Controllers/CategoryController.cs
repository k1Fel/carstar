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
                if (string.IsNullOrEmpty(createCategoryDto.name))
                {
                    return BadRequest(new { message = "Назва категорії не може бути порожньою" });
                }
                if(await _categoryService.CategoryExists(createCategoryDto.name) != null)
                {
                    return BadRequest(new { message = "Категорія з такою назвою вже існує" });
                }
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
            if (id <= 0)
            {
                return BadRequest(new { message = "Невірний ID категорії" });
            }
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound(new { message = "Категорія не знайдена" });
            }
            return Ok(category.ToCategoryResponseDto());
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
                if (!categories.Any())
                {
                    return NotFound(new { message = "Категорії не знайдено" });
                }   
                var response = categories.Select(c => c.ToCategoryResponseDto());
                return Ok(response);
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
                if (category == null)
                {
                    return NotFound(new { message = "Категорія не знайдена" });
                }
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
            var result = await _categoryService.DeleteCategory(id);
            if (!result)
            {
                return NotFound(new { message = "Категорія не знайдена" });
            }
            return Ok(new { message = "Категорія успішно видалена" });
        }
    }
}