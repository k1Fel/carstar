using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(int accountId);
        Task<Cart> AddToCartAsync(int accountId, int productId, int quantity);
        Task<Cart> UpdateCartItemAsync(int accountId, int productId, int quantity);
        Task<Cart?> DeleteCartItemAsync(int accountId, int productId);
        Task<Cart> ClearCartAsync(int accountId);
    }
}