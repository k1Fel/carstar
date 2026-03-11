using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Category;
using api.Models;

namespace api.Mappers.CategoryMappers
{
    public static class CategoryMapper
    {
        public static Category ToCategoryDto(this UpdateCategoryDto updateCategoryDto)
        {
            return new Category
            {
                Id = updateCategoryDto.Id,
                Name = updateCategoryDto.Name
            };
        }
        public static Category ToCategory(this CreateCategoryDto createCategoryDto)
        {
            return new Category
            {
                
                Name = createCategoryDto.name
            };
        }
        public static CategoryDtoResponse ToCategoryResponseDto(this Category category)
        {
            return new CategoryDtoResponse
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = category.Products?.Count ?? 0
            };
        }
    }
}