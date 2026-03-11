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
            var category = await _categoryService.CreateCategory(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category.ToCategoryResponseDto());
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            var response = categories.Select(c => c.ToCategoryResponseDto());
            return Ok(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryService.UpdateCategory(id, updateCategoryDto);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}