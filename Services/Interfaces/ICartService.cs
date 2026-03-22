using System.Threading.Tasks;
using api.DTO.Cart;

namespace api.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> AddToCartAsync(int accountId, AddToCartDto addToCartDto); 
        Task<CartResponseDto> GetCartAsync(int accountId);
        Task<CartResponseDto> UpdateCartItemAsync(int accountId, UpdateCartItemDto updateCartItemDto); 
        Task<CartResponseDto> DeleteCartItemAsync(int accountId, int cartItemId);
        Task<CartResponseDto> ClearCartAsync(int accountId);
    }
}