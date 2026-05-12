using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "part_type", "brand", "model", "country"
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category>? Children { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}