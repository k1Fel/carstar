using System;
using System.Collections.Generic;
using System.Linq;
using api.Models;
using api.DTO.Cart;

namespace api.Mappers.CartMapper
{
    public static class CartMapper
    {
        // ✅ Cart → CartResponseDto (для відповіді)
        public static CartResponseDto ToCartResponseDto(this Cart cart)
        {
            var items = cart.CartItems?.Select(ci => ci.ToCartItemResponseDto()).ToList() 
                ?? new List<CartItemResponseDto>();

            return new CartResponseDto
            {
                Id = cart.Id,
                AccountId = cart.AccountId,
                ItemsCount = items.Count,
                TotalAmount = (int)items.Sum(i => i.Subtotal), // ✅ Рахуємо суму
                Items = items
            };
        }

        // ✅ CartItem → CartItemResponseDto (для відповіді)
        public static CartItemResponseDto ToCartItemResponseDto(this CartItem cartItem)
        {
            var price = cartItem.Product?.Price ?? 0;

            return new CartItemResponseDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product?.Name ?? "Unknown",
                Price = price,
                Quantity = cartItem.Quantity,
                Subtotal = price * cartItem.Quantity
            };
        }

        // ✅ AddToCartDto → CartItem (для створення)
        public static CartItem ToCartItem(this AddToCartDto addToCartDto, int cartId)
        {
            return new CartItem
            {
                CartId = cartId,
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity
            };
        }
    }
}