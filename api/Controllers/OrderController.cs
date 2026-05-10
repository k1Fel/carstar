using System;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DTO.Order;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
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
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                int accountId = GetAccountId();

                var order = await _orderService.CreateOrderFromCartAsync(accountId, createOrderDto);

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                int accountId = GetAccountId();

                var order = await _orderService.GetOrderByIdAsync(id, accountId);

                if (order == null)
                {
                    return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
                }

                return Ok(order);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                int accountId = GetAccountId();

                var orders = await _orderService.GetOrdersByAccountIdAsync(accountId);

                return Ok(orders);
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

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpPatch("{id:int}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto.Status);

                if (order == null)
                {
                    return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }

        [HttpDelete("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
               
                int accountId = GetAccountId();

                bool result = await _orderService.CancelOrderAsync(id, accountId);

                if (!result)
                {
                    return NotFound(new { message = $"Замовлення з ID {id} не знайдено" });
                }

                return Ok(new { message = "Замовлення успішно скасовано" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Внутрішня помилка сервера" });
            }
        }
    }
}