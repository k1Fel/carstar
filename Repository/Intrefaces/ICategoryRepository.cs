using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repository.Intrefaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategory(Category category);
        Task<Category?> GetCategoryById(int id);
        Task<List<Category>> GetAllCategories();
        Task<Category?> UpdateCategory(int id, Category category);
        Task<bool> DeleteCategory(int id);
        Task<Category?> GetCategoryByName(string name);
    }
}