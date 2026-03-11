using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Cart
{
    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}