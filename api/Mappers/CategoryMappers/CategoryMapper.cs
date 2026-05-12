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
                Name = updateCategoryDto.Name,
                Type = updateCategoryDto.Type,
                ParentId = updateCategoryDto.ParentId
            };
        }
        public static CategoryDtoResponse ToCategoryResponseDto(this Category category)
        {
            return new CategoryDtoResponse
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                ParentId = category.ParentId,
                ProductCount = category.ProductCategories?.Count ?? 0,
                Children = category.Children?
                    .Select(c => c.ToCategoryResponseDto())
                    .ToList()
            };
}

        public static Category ToCategory(this CreateCategoryDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Type = dto.Type,
                ParentId = dto.ParentId
            };
        }
        
    }
}