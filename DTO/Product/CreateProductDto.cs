using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO
{
    public class CreateProductDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Назва товару обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Опис товару обов'язковий")]
        [StringLength(500, ErrorMessage = "Опис не може перевищувати 500 символів")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ціна обов'язкова")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути більшою за 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Кількість на складі обов'язкова")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "ID категорії обов'язковий")]
        [Range(1, int.MaxValue, ErrorMessage = "Невірний ID категорії")]
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }
}