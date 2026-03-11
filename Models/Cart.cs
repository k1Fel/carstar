using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        
    }
}