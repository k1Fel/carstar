using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.DTO.Category;
namespace api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<Category?> GetCategoryById(int id);
        Task<List<Category>> GetAllCategories();
        Task<Category?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(int id);
    }
}