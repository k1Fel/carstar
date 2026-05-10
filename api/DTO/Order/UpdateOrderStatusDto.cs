using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Order
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}