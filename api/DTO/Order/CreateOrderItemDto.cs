using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace api.DTO.Order
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "ID продукту обов'язковий")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Кількість обов'язкова")]
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}