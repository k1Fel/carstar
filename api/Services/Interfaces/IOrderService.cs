using System.Collections.Generic;
using System.Threading.Tasks;
using api.DTO.Order;

namespace api.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseOrderDto> CreateOrderFromCartAsync(int accountId, CreateOrderDto createOrderDto); 
        Task<ResponseOrderDto?> GetOrderByIdAsync(int orderId, int accountId); 
        Task<List<ResponseOrderDto>> GetOrdersByAccountIdAsync(int accountId); 
        Task<List<ResponseOrderDto>> GetAllOrdersAsync(); 
        Task<ResponseOrderDto?> UpdateOrderStatusAsync(int orderId, string status); 
        Task<bool> CancelOrderAsync(int orderId, int accountId);
    }
}