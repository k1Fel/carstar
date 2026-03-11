using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTO.Order;
using api.Mappers.OrderMappers;
using api.Models;
using api.Repository.Intrefaces;
using api.Services.Interfaces;

namespace api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ICartRepository cartRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartRepository = cartRepository;

        }
        public async Task<Order> CreateOrder(Order order)
        {
            return await _orderRepository.CreateOrderAsync(order);
        }

        public async Task<Order> CreateOrderFromCart(int accountId, CreateOrderDto orderDto)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null|| cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("Cart is empty or does not exist.");
            }

            var order = orderDto.ToOrder();
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Product with ID {cartItem.ProductId} does not exist.");
                }
                var orderItem = cartItem.ToOrderItem(order.Id, product.Price);
                order.OrderItems.Add(orderItem);
                product.Stock -= cartItem.Quantity; // Update stock
                await _productRepository.UpdateProductAsync(product.Id, product);
                var orderItems = cartItem.ToOrderItem(order.Id, product.Price);
                order.OrderItems.Add(orderItems);
            }
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            var createdOrder = await _orderRepository.CreateOrderAsync(order);
            await _cartRepository.ClearCartAsync(cart.Id); // Clear cart after order creation

            return createdOrder;
        }

        public async Task<bool> DeleteOrder(int id)
        {
            return await _orderRepository.CancelOrderAsync(id);
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _orderRepository.GetAllOrdersAsync();

        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<Order?> UpdateOrder(int id, Order order)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.Status = order.Status;
            await _orderRepository.UpdateOrderAsync(existingOrder);
            return existingOrder;
        }
    }
}