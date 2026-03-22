using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace api.DTO
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва має бути від 3 до 100 символів")]
        public string Name{ get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Опис не може перевищувати 500 символів")]
        public string Description{ get; set; } = string.Empty;
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути більшою за 0")]

        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Кількість не може бути від'ємною")]
        public int Stock { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Невірний ID категорії")]
        public int CategoryId { get; set; }
    }
}