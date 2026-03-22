using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace api.DTO.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "ID користувача обов'язковий")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Загальна сума обов'язкова")]
        public decimal TotalAmount { get; set; }
        
        public string Status { get; set; } = string.Empty; // "Pending", "Completed", "Cancelled"
        [Required(ErrorMessage = "Адреса доставки обов'язкова")]
        public string ShippingAddress { get; set; } = string.Empty;
        
        public List<CreateOrderItemDto>? OrderItems { get; set; }
    }
}