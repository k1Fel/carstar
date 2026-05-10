using System.Threading.Tasks;
using api.Models;

namespace api.Repository.Intrefaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByAccountId(int accountId);
        Task<Cart> CreateCart(Cart cart);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId); 
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem?> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartItemAsync(int cartItemId);
        Task<Cart> ClearCartAsync(int cartId);
    }
}