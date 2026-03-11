using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Cart;
using api.Mappers.CartMapper;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace api.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetCart(int accountId)
        {
            try
            {
                var cart = await _cartService.GetCartAsync(accountId);
                if (cart == null)
                {
                    return NotFound();
                }
                return Ok(cart.ToCartResponseDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }       
        }
        [HttpPost("{accountId}/add")]
        public async Task<IActionResult> AddToCart(int accountId, [FromBody] AddToCartDto request)
        {
            try
            {
                var cart = await _cartService.AddToCartAsync(accountId, request.ProductId, request.Quantity);
                return Ok(cart.ToCartResponseDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }   
        }
        [HttpPut("{accountId}/update")]
        public async Task<IActionResult> UpdateCartItem(int accountId, [FromBody] UpdateCartItemDto request)
        {
            try
            {
                var cart = await _cartService.UpdateCartItemAsync(accountId, request.CartItemId, request.Quantity);
                return Ok(cart.ToCartResponseDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete("{accountId}/delete/{productId}")]
        public async Task<IActionResult> DeleteCartItem(int accountId, int productId)
        {
            try
            {
                 var cart = await _cartService.DeleteCartItemAsync(accountId, productId);
                if (cart == null)
                {
                    return NotFound();
                }
                return Ok(cart.ToCartResponseDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpDelete("{accountId}/clear")]
        public async Task<IActionResult> ClearCart(int accountId)
        {
            try
            {
                var cart = await _cartService.ClearCartAsync(accountId);
                return Ok(cart.ToCartResponseDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}