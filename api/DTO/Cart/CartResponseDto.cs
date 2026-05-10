using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace api.DTO.Cart
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int ItemsCount { get; set; }
        public int TotalAmount { get; set; }
        public ICollection<CartItemResponseDto>? Items { get; set; }
    }
}