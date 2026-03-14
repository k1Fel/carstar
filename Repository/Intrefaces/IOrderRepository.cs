using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Repository.Intrefaces
{
     public interface IOrderRepository
    {
        // READ
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByAccountIdAsync(int accountId);
        Task<List<Order>> GetAllOrdersAsync();

        // CREATE
        Task<Order> CreateOrderAsync(Order order);

        // UPDATE
        Task<Order?> UpdateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

        // CANCEL/DELETE
        Task<bool> CancelOrderAsync(int orderId);
    }
}