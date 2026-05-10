using System;
using System.Collections.Generic;
using System.Linq;
using api.DTO.Order;
using api.Models;

namespace api.Mappers.OrderMappers
{
    public static class OrderMapper
    {
        public static Order ToOrder(this CreateOrderDto dto)
        {
            return new Order
            {
                AccountId = dto.AccountId,
                ShippingAddress = dto.ShippingAddress,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };
        }

        public static ResponseOrderDto ToOrderResponseDto(this Order order)
        {
            return new ResponseOrderDto
            {
                Id = order.Id,
                AccountId = order.AccountId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                CreatedAt = order.CreatedAt,
                OrderItems = order.OrderItems?.Select(oi => oi.ToOrderItemResponseDto()).ToList() 
                    ?? new List<OrderItemResponseDto>()
            };
        }

        public static OrderItemResponseDto ToOrderItemResponseDto(this OrderItem orderItem)
        {
            return new OrderItemResponseDto
            {
                Id = orderItem.Id,
                ProductId = orderItem.ProductId,
                Price = orderItem.Price,
                Quantity = orderItem.Quantity,
            };
        }

        public static OrderItem ToOrderItem(this CartItem cartItem, int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product?.Price ?? 0  
            };
        }

        public static OrderItem ToOrderItem(this CreateOrderItemDto dto, int orderId, decimal price)
        {
            return new OrderItem
            {
                OrderId = orderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = price  
            };
        }
    }
}