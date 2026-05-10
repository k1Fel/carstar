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
        Task<CategoryDtoResponse> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<CategoryDtoResponse?> GetCategoryById(int id);
        Task<List<CategoryDtoResponse>> GetAllCategories();
        Task<CategoryDtoResponse?> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(int id);
    }
}