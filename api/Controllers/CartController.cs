using System;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DTO.Cart;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize] // Тільки авторизовані користувачі
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetAccountId()
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                throw new UnauthorizedAccessException("Невірний токен");
            }
            return accountId;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            try
            {
                int accountId = GetAccountId();
                var cart = await _cartService.AddToCartAsync(accountId, addToCartDto);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                int accountId = GetAccountId();
                var cart = await _cartService.GetCartAsync(accountId);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

      
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                int accountId = GetAccountId();
                var cart = await _cartService.UpdateCartItemAsync(accountId, updateCartItemDto);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

       
        [HttpDelete("{cartItemId:int}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            try
            {
                int accountId = GetAccountId();
                var cart = await _cartService.DeleteCartItemAsync(accountId, cartItemId);
                return Ok(cart);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                int accountId = GetAccountId();
                var cart = await _cartService.ClearCartAsync(accountId);
                return Ok(new { message = "Кошик успішно очищено", cart });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }
    }
}