using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace api.DTO.Category
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;
    }
}