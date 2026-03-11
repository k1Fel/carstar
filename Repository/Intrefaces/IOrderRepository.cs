using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repository.Intrefaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByAccountIdAsync(int accountId);
        Task<List<Order>> GetAllOrdersAsync();
        
        Task<Order> CreateOrderAsync(Order order);
        
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int orderId, string status);

        Task<bool> CancelOrderAsync(int orderId);

    }
}