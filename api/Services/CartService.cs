using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Cart;
using api.Mappers.CartMapper;
using api.Models;
using api.Repository.Intrefaces;
using api.Services.Interfaces;

namespace api.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartResponseDto> AddToCartAsync(int accountId, AddToCartDto addToCartDto)
        {
            // 1️⃣ ПЕРЕВІРКА ІСНУВАННЯ ТОВАРУ
            var product = await _productRepository.GetProductByIdAsync(addToCartDto.ProductId);
            if (product == null)
            {
                throw new ArgumentException($"Товар з ID {addToCartDto.ProductId} не знайдено");
            }

            // 2️⃣ ПЕРЕВІРКА НАЯВНОСТІ НА СКЛАДІ
            if (product.Stock < addToCartDto.Quantity)
            {
                throw new ArgumentException($"Недостатня кількість товару. Доступно: {product.Stock}");
            }

            // 3️⃣ ОТРИМАННЯ АБО СТВОРЕННЯ КОШИКА
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                cart = new Cart { AccountId = accountId };
                cart = await _cartRepository.CreateCart(cart);
            }

            // 4️⃣ ПЕРЕВІРКА ЧИ ТОВАР ВЖЕ Є В КОШИКУ
            var existingCartItem = await _cartRepository.GetCartItemAsync(cart.Id, addToCartDto.ProductId);
            
            if (existingCartItem != null)
            {
                // ОНОВЛЮЄМО КІЛЬКІСТЬ
                int newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;
                
                if (product.Stock < newQuantity)
                {
                    throw new ArgumentException($"Недостатня кількість товару. Доступно: {product.Stock}");
                }
                
                existingCartItem.Quantity = newQuantity;
                await _cartRepository.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                // ДОДАЄМО НОВИЙ ТОВАР
                var cartItem = addToCartDto.ToCartItem(cart.Id);
                await _cartRepository.AddCartItemAsync(cartItem);
            }
            
            // 5️⃣ ОТРИМУЄМО ОНОВЛЕНИЙ КОШИК І ПЕРЕТВОРЮЄМО В DTO
            var updatedCart = await _cartRepository.GetCartByAccountId(accountId);
            
            if (updatedCart == null)
            {
                throw new InvalidOperationException("Помилка при отриманні оновленого кошика");
            }

            return updatedCart.ToCartResponseDto();
        }

        // ✅ ПОВЕРТАЄ CartResponseDto
        public async Task<CartResponseDto> GetCartAsync(int accountId)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            
            // ЯКЩО КОШИКА НЕМАЄ - СТВОРЮЄМО ПОРОЖНІЙ
            if (cart == null)
            {
                cart = new Cart 
                { 
                    AccountId = accountId,
                    CartItems = new List<CartItem>()
                };
                cart = await _cartRepository.CreateCart(cart);
            }
            
            // MAPPER В СЕРВІСІ
            return cart.ToCartResponseDto();
        }

        // ✅ ПОВЕРТАЄ CartResponseDto
        public async Task<CartResponseDto> UpdateCartItemAsync(int accountId, UpdateCartItemDto updateCartItemDto)
        {
            // 1️⃣ ПЕРЕВІРКА ІСНУВАННЯ КОШИКА
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new ArgumentException("Кошик не знайдено");
            }

            // 2️⃣ ПЕРЕВІРКА ІСНУВАННЯ ТОВАРУ В КОШИКУ
            var cartItem = await _cartRepository.GetCartItemByIdAsync(updateCartItemDto.CartItemId);
            
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                throw new ArgumentException("Товар в кошику не знайдено або не належить цьому користувачу");
            }

            // 3️⃣ ЯКЩО КІЛЬКІСТЬ 0 АБО МЕНШЕ - ВИДАЛЯЄМО
            if (updateCartItemDto.Quantity <= 0)
            {
                await _cartRepository.DeleteCartItemAsync(cartItem.Id);
            }
            else
            {
                // 4️⃣ ПЕРЕВІРКА НАЯВНОСТІ НА СКЛАДІ
                var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Товар з ID {cartItem.ProductId} не знайдено");
                }

                if (product.Stock < updateCartItemDto.Quantity)
                {
                    throw new ArgumentException($"Недостатня кількість товару. Доступно: {product.Stock}");
                }

                // 5️⃣ ОНОВЛЮЄМО КІЛЬКІСТЬ
                cartItem.Quantity = updateCartItemDto.Quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
            
            // 6️⃣ ПОВЕРТАЄМО ОНОВЛЕНИЙ КОШИК
            var updatedCart = await _cartRepository.GetCartByAccountId(accountId);
            
            if (updatedCart == null)
            {
                throw new InvalidOperationException("Помилка при отриманні оновленого кошика");
            }

            return updatedCart.ToCartResponseDto();
        }

        // ✅ ПОВЕРТАЄ CartResponseDto
        public async Task<CartResponseDto> DeleteCartItemAsync(int accountId, int cartItemId)
        {
            // 1️⃣ ПЕРЕВІРКА ІСНУВАННЯ КОШИКА
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new ArgumentException("Кошик не знайдено");
            }

            // 2️⃣ ПЕРЕВІРКА ІСНУВАННЯ ТОВАРУ В КОШИКУ
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                throw new ArgumentException("Товар в кошику не знайдено або не належить цьому користувачу");
            }

            // 3️⃣ ВИДАЛЕННЯ
            await _cartRepository.DeleteCartItemAsync(cartItem.Id);
            
            // 4️⃣ ПОВЕРТАЄМО ОНОВЛЕНИЙ КОШИК
            var updatedCart = await _cartRepository.GetCartByAccountId(accountId);
            
            if (updatedCart == null)
            {
                // Якщо кошик порожній після видалення - створюємо порожній DTO
                return new Cart 
                { 
                    AccountId = accountId,
                    CartItems = new List<CartItem>()
                }.ToCartResponseDto();
            }

            return updatedCart.ToCartResponseDto();
        }

        // ✅ ПОВЕРТАЄ CartResponseDto
        public async Task<CartResponseDto> ClearCartAsync(int accountId)
        {
            // 1️⃣ ПЕРЕВІРКА ІСНУВАННЯ КОШИКА
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new ArgumentException("Кошик не знайдено");
            }

            // 2️⃣ ОЧИЩЕННЯ
            await _cartRepository.ClearCartAsync(cart.Id);
            
            // 3️⃣ ПОВЕРТАЄМО ПОРОЖНІЙ КОШИК
            var clearedCart = await _cartRepository.GetCartByAccountId(accountId);
            
            if (clearedCart == null)
            {
                // Створюємо порожній кошик
                clearedCart = new Cart 
                { 
                    AccountId = accountId,
                    CartItems = new List<CartItem>()
                };
            }

            return clearedCart.ToCartResponseDto();
        }
    }
}