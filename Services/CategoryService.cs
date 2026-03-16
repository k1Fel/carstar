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
        public async Task<Category> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var category = createCategoryDto.ToCategory();
            await _categoryRepository.CreateCategory(category);
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null) return false;
            await _categoryRepository.DeleteCategory(id);
            return true;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryRepository.GetAllCategories();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _categoryRepository.GetCategoryById(id);
        }

        public async Task<Category?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null) return null;
            var updatedCategory = updateCategoryDto.ToCategoryDto();
            await _categoryRepository.UpdateCategory(id, updatedCategory);
            return updatedCategory;
        }
        public async Task<Category?> CategoryExists(string name)
        {
            var category = await _categoryRepository.GetCategoryByName(name);
            return category;
        }
    }
}