using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Cart
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}