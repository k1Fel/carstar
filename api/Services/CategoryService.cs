using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Category;
using api.Mappers.CategoryMappers;
using api.Models;
using api.Services.Interfaces;
using api.Repository.Intrefaces;

namespace api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        } 
        public async Task<CategoryDtoResponse> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByName(createCategoryDto.name);
            if (existingCategory != null)            {
                throw new ArgumentException("Категорія з такою назвою вже існує");
            }
            var category = createCategoryDto.ToCategory();
            var createdCategory = await _categoryRepository.CreateCategory(category);
            return createdCategory.ToCategoryResponseDto();
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null) return false;
            await _categoryRepository.DeleteCategory(id);
            return true;
        }

        public async Task<List<CategoryDtoResponse>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllCategories();
            return categories.Select(c => c.ToCategoryResponseDto()).ToList();
        }

        public async Task<CategoryDtoResponse?> GetCategoryById(int id)
        {
            
            
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null) return null;
            return category?.ToCategoryResponseDto();
        }

        public async Task<CategoryDtoResponse?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null) return null;

            var updatedCategory = updateCategoryDto.ToCategoryDto();

            if (updatedCategory != null && updatedCategory.Id != id)
            {
                throw new ArgumentException("Категорія з такою назвою вже існує");
            }
            var update = await _categoryRepository.UpdateCategory(id, updatedCategory);
            return update?.ToCategoryResponseDto();
        }
    }
}