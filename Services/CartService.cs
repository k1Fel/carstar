using System;
using System.Threading.Tasks;
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


        public async Task<Cart> AddToCartAsync(int accountId, int productId, int quantity)
        {

            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Товар не знайдено");
            }

            if (product.Stock < quantity)
            {
                throw new Exception($"Недостатня кількість товару. Доступно: {product.Stock}");
            }


            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                cart = new Cart { AccountId = accountId };
                cart = await _cartRepository.CreateCart(cart);
            }

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem != null)
            {
  
                if (product.Stock < cartItem.Quantity + quantity)
                {
                    throw new Exception($"Недостатня кількість товару. Доступно: {product.Stock}");
                }
                
                cartItem.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
            else
            {
    
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _cartRepository.AddCartItemAsync(cartItem);
            }
            
            // Повертаємо оновлений кошик
            return await _cartRepository.GetCartByAccountId(accountId) ?? cart;
        }

        public async Task<Cart?> GetCartAsync(int accountId)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            
            // Якщо кошика немає - створюємо порожній
            if (cart == null)
            {
                cart = new Cart 
                { 
                    AccountId = accountId,
                    CartItems = new List<CartItem>()
                };
            }
            
            return cart;
        }


        public async Task<Cart> UpdateCartItemAsync(int accountId, int cartItemId, int quantity)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new Exception("Кошик не знайдено");
            }

  
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            
            // Перевіряємо що CartItem існує і належить цьому кошику
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                throw new Exception("Товар в кошику не знайдено або не належить цьому користувачу");
            }


            if (quantity <= 0)
            {
                await _cartRepository.DeleteCartItemAsync(cartItem.Id);
            }
            else
            {
                // Перевірка Stock
                var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product != null && product.Stock < quantity)
                {
                    throw new Exception($"Недостатня кількість товару. Доступно: {product.Stock}");
                }

                cartItem.Quantity = quantity;
                await _cartRepository.UpdateCartItemAsync(cartItem);
            }
            
            return await _cartRepository.GetCartByAccountId(accountId) ?? cart;
        }


        public async Task<Cart?> DeleteCartItemAsync(int accountId, int productId)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new Exception("Кошик не знайдено");
            }

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
            {
                throw new Exception("Товар в кошику не знайдено");
            }

            await _cartRepository.DeleteCartItemAsync(cartItem.Id);
            return await _cartRepository.GetCartByAccountId(accountId);
        }


        public async Task<Cart> ClearCartAsync(int accountId)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null)
            {
                throw new Exception("Кошик не знайдено");
            }

            return await _cartRepository.ClearCartAsync(cart.Id);
        }
    }
}