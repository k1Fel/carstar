using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTO.Category
{
    public class CategoryDtoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}