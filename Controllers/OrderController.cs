using System;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Order;
using api.Services.Interfaces;
using api.Mappers.OrderMappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/orders
        // Створити замовлення з кошика
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                if (dto == null || dto.AccountId <= 0 || string.IsNullOrEmpty(dto.ShippingAddress))
                {
                    return BadRequest(new { message = "Невірні дані для створення замовлення" });
                }
                if(dto.TotalAmount <= 0)
                {
                    return BadRequest(new { message = "Загальна сума замовлення повинна бути більше нуля" });
                }
                
                var order = await _orderService.CreateOrderFromCartAsync(
                    dto.AccountId,
                    dto.ShippingAddress
                );

                return Ok(order.ToOrderResponseDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders/{id}?accountId={accountId}
        // Отримати замовлення за ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id, [FromQuery] int accountId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id, accountId);
                if (id <= 0)
                {
                    return BadRequest(new { message = "Невірний ID замовлення" });
                }
                if (accountId <= 0)
                {
                    return BadRequest(new { message = "Невірний ID користувача" });
                }
                if (order == null)
                {
                    return NotFound(new { message = "Замовлення не знайдено" });
                }
                
                return Ok(order.ToOrderResponseDto());
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders/user/{accountId}
        // Отримати всі замовлення користувача
        [HttpGet("user/{accountId}")]
        public async Task<IActionResult> GetUserOrders(int accountId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByAccountIdAsync(accountId);
                if (accountId <= 0)
                {
                    return BadRequest(new { message = "Невірний ID користувача" });
                }
                var ordersDto = orders.Select(o => o.ToOrderResponseDto()).ToList();
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders/all
        // Отримати всі замовлення (для адміна)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { message = "Замовлення не знайдено" });
                }
               
                
                var ordersDto = orders.Select(o => o.ToOrderResponseDto()).ToList();
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/orders/{id}/status
        // Оновити статус замовлення (адмін)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
                if (order == null)
                {
                    return NotFound(new { message = "Замовлення не знайдено" });
                }

                return Ok(order.ToOrderResponseDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/orders/{id}/cancel?accountId={accountId}
        // Скасувати замовлення (користувач)
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id, [FromQuery] int accountId)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id, accountId);
                if (!result)
                {
                    return NotFound(new { message = "Замовлення не знайдено" });
                }

                return Ok(new { message = "Замовлення успішно скасовано" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}