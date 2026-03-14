using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Order;
using api.Models;

namespace api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderFromCartAsync(int accountId, string shippingAddress);
        Task<Order?> GetOrderByIdAsync(int orderId, int accountId);
        Task<List<Order>> GetOrdersByAccountIdAsync(int accountId);
        Task<List<Order>> GetAllOrdersAsync(); 
        Task<Order?> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId, int accountId);
    }
}