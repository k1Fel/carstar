using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty; // "Pending", "Completed", "Cancelled"
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}