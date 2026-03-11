using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Order
{
    public class CreateOrderDto
    {
        public int AccountId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty; // "Pending", "Completed", "Cancelled"
        public string ShippingAddress { get; set; } = string.Empty;
        public List<CreateOrderItemDto>? OrderItems { get; set; }
    }
}