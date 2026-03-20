using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO
{
    public class CreateProductDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Name{ get; set; } = string.Empty;
        
        public string Description{ get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}