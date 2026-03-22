using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace api.DTO.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Назва категорії обов'язкова")]
        public string name { get; set; } = string.Empty;
    }
}
