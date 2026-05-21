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
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<ResponseOrderDto> CreateOrderFromCartAsync(int accountId, CreateOrderDto createOrderDto)
        {
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("Кошик порожній");
            }

            var order = new Order
            {
                AccountId = accountId,
                ShippingAddress = createOrderDto.ShippingAddress,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                // ПЕРЕВІРИТИ ЩО ТОВАР ІСНУЄ
                var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Товар з ID {cartItem.ProductId} не знайдено");
                }

                // ПЕРЕВІРИТИ STOCK
                if (product.Stock < cartItem.Quantity)
                {
                    throw new ArgumentException($"Недостатня кількість товару: {product.Name}. Доступно: {product.Stock}, замовлено: {cartItem.Quantity}");
                }

                // ЗМЕНШИТИ STOCK
                product.Stock -= cartItem.Quantity;
                await _productRepository.UpdateProductAsync(product.Id, product, new List<int>());

                // СТВОРИТИ OrderItem (використовуємо mapper)
                var orderItem = cartItem.ToOrderItem(order.Id);
                orderItem.Price = product.Price; // Зберігаємо ціну на момент замовлення
                
                order.OrderItems.Add(orderItem);

                totalAmount += orderItem.Price * orderItem.Quantity;
            }

            order.TotalAmount = totalAmount;

            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            await _cartRepository.ClearCartAsync(cart.Id);

            // 7️⃣ ПОВЕРНУТИ DTO (mapper в сервісі)
            return createdOrder.ToOrderResponseDto();
        }

        // ✅ ПОВЕРТАЄ ResponseOrderDto
        public async Task<ResponseOrderDto?> GetOrderByIdAsync(int orderId, int accountId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return null;
            }

            // ПЕРЕВІРКА ЩО ЗАМОВЛЕННЯ НАЛЕЖИТЬ КОРИСТУВАЧУ
            if (order.AccountId != accountId)
            {
                throw new UnauthorizedAccessException("Ви не маєте доступу до цього замовлення");
            }

            // MAPPER В СЕРВІСІ
            return order.ToOrderResponseDto();
        }

        // ✅ ПОВЕРТАЄ List<ResponseOrderDto>
        public async Task<List<ResponseOrderDto>> GetOrdersByAccountIdAsync(int accountId)
        {
            var orders = await _orderRepository.GetOrdersByAccountIdAsync(accountId);
            
            // MAPPER В СЕРВІСІ
            return orders.Select(o => o.ToOrderResponseDto()).ToList();
        }

        // ✅ ПОВЕРТАЄ List<ResponseOrderDto> (для адміна)
        public async Task<List<ResponseOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            
            // MAPPER В СЕРВІСІ
            return orders.Select(o => o.ToOrderResponseDto()).ToList();
        }

        // ✅ ПОВЕРТАЄ ResponseOrderDto
       public async Task<ResponseOrderDto?> UpdateOrderStatusAsync(int orderId, string status, string? cancellationReason = null)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) return null;

            var validTransitions = new Dictionary<string, List<string>>
            {
                ["Pending"]    = new() { "Processing", "Cancelled" },
                ["Processing"] = new() { "Shipped", "Cancelled" },
                ["Shipped"]    = new() { "Delivered" },
                ["Delivered"]  = new(),
                ["Cancelled"]  = new()
            };

            if (!validTransitions.ContainsKey(order.Status))
                throw new ArgumentException($"Невідомий статус: {order.Status}");

            if (!validTransitions[order.Status].Contains(status))
                throw new ArgumentException($"Неможливо змінити статус з '{order.Status}' на '{status}'");

            if (status == "Cancelled")
            {
                order.CancellationReason = cancellationReason;
                foreach (var item in order.OrderItems ?? new())
                {
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _productRepository.UpdateProductAsync(product.Id, product, new List<int>());
                    }
                }
            }

            await _orderRepository.UpdateOrderStatusAsync(orderId, status);

            // Зберегти причину відмови
            if (status == "Cancelled" && cancellationReason != null)
            {
                order.Status = status;
                await _orderRepository.UpdateOrderAsync(order);
            }

            var updated = await _orderRepository.GetOrderByIdAsync(orderId);
            return updated?.ToOrderResponseDto();
        }

        // ✅ ПОВЕРТАЄ BOOL
        public async Task<bool> CancelOrderAsync(int orderId, int accountId)
        {
            // 1️⃣ ПЕРЕВІРКА ІСНУВАННЯ ЗАМОВЛЕННЯ
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Замовлення з ID {orderId} не знайдено");
            }

            // 2️⃣ ПЕРЕВІРКА ЩО ЗАМОВЛЕННЯ НАЛЕЖИТЬ КОРИСТУВАЧУ
            if (order.AccountId != accountId)
            {
                throw new UnauthorizedAccessException("Ви не маєте доступу до цього замовлення");
            }

            // 3️⃣ МОЖНА СКАСУВАТИ ТІЛЬКИ PENDING ЗАМОВЛЕННЯ
            if (order.Status != "Pending")
            {
                throw new ArgumentException($"Неможливо скасувати замовлення зі статусом '{order.Status}'. Можна скасувати тільки замовлення зі статусом 'Pending'");
            }

            // 4️⃣ ПОВЕРТАЄМО ТОВАРИ НА СКЛАД
            foreach (var orderItem in order.OrderItems ?? new List<OrderItem>())
            {
                var product = await _productRepository.GetProductByIdAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.Stock += orderItem.Quantity;
                    await _productRepository.UpdateProductAsync(product.Id, product, new List<int>());
                }
            }

            // 5️⃣ СКАСУВАТИ ЗАМОВЛЕННЯ
            return await _orderRepository.CancelOrderAsync(orderId);
        }
    }
}