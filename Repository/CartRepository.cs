using System;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.Repository.Intrefaces;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Отримати кошик з товарами
        public async Task<Cart?> GetCartByAccountId(int accountId)
        {
            return await _context.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
        }

        // ✅ Створити кошик
        public async Task<Cart> CreateCart(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // ✅ Знайти CartItem за CartId і ProductId (для додавання)
        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        // ✅ ДОДАЙТЕ ЦЕЙ МЕТОД - Знайти CartItem за його Id (для оновлення)
        public async Task<CartItem?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        // ✅ Додати товар
        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        // ✅ Оновити товар
        public async Task<CartItem?> UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        // ✅ Видалити товар
        public async Task<bool> DeleteCartItemAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Очистити кошик
        public async Task<Cart> ClearCartAsync(int cartId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            // Повертаємо оновлений кошик
            var cart = await _context.Carts
                .Include(c => c.CartItems!)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            return cart ?? throw new Exception("Cart not found");
        }
    }
}