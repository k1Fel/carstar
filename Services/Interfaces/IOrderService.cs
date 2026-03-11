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
        Task<Order> CreateOrder(Order order);
        Task<Order?> GetOrderById(int id);
        Task<List<Order>> GetAllOrders();
        Task<Order?> UpdateOrder(int id, Order order);
        Task<bool> DeleteOrder(int id);
        Task<Order> CreateOrderFromCart(int accountId, CreateOrderDto orderDto);
    }
}