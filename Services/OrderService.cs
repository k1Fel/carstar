using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Repository.Intrefaces;
using api.Services.Interfaces;
using api.Mappers.OrderMappers;

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

        // ✅ Створити замовлення з кошика
        public async Task<Order> CreateOrderFromCartAsync(int accountId, string shippingAddress)
        {
            // 1. Отримати кошик
            var cart = await _cartRepository.GetCartByAccountId(accountId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new Exception("Кошик порожній");
            }

            // 2. Створити Order
            var order = new Order
            {
                AccountId = accountId,
                ShippingAddress = shippingAddress,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            // 3. Копіювати CartItems в OrderItems
            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                // Перевірити що товар існує
                var product = await _productRepository.GetProductByIdAsync(cartItem.ProductId);
                if (product == null)
                {
                    throw new Exception($"Товар з ID {cartItem.ProductId} не знайдено");
                }

                // Перевірити Stock
                if (product.Stock < cartItem.Quantity)
                {
                    throw new Exception($"Недостатня кількість товару: {product.Name}. Доступно: {product.Stock}, замовлено: {cartItem.Quantity}");
                }

                // Зменшити Stock
                product.Stock -= cartItem.Quantity;
                await _productRepository.UpdateProductAsync(product.Id, product);

                // Створити OrderItem (використовуємо mapper)
                var orderItem = cartItem.ToOrderItem(order.Id);
                order.OrderItems.Add(orderItem);

                totalAmount += orderItem.Price * orderItem.Quantity;
            }

            // 4. Встановити TotalAmount
            order.TotalAmount = totalAmount;

            // 5. Зберегти Order
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // 6. Очистити кошик
            await _cartRepository.ClearCartAsync(cart.Id);

            return createdOrder;
        }

        // ✅ Отримати замовлення за ID
        public async Task<Order?> GetOrderByIdAsync(int orderId, int accountId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            // Перевірка що замовлення належить користувачу
            if (order != null && order.AccountId != accountId)
            {
                throw new UnauthorizedAccessException("Ви не маєте доступу до цього замовлення");
            }

            return order;
        }

        // ✅ Отримати всі замовлення користувача
        public async Task<List<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
            return await _orderRepository.GetOrdersByAccountIdAsync(accountId);
        }

        // ✅ Отримати всі замовлення (для адміна)
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        // ✅ Оновити статус замовлення
        public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            // Валідація переходів статусів
            var validTransitions = new Dictionary<string, List<string>>
            {
                ["Pending"] = new List<string> { "Processing", "Cancelled" },
                ["Processing"] = new List<string> { "Shipped", "Cancelled" },
                ["Shipped"] = new List<string> { "Delivered" },
                ["Delivered"] = new List<string>(),
                ["Cancelled"] = new List<string>()
            };

            if (!validTransitions.ContainsKey(order.Status))
            {
                throw new Exception($"Невідомий статус: {order.Status}");
            }

            if (!validTransitions[order.Status].Contains(status))
            {
                throw new Exception($"Неможливо змінити статус з '{order.Status}' на '{status}'");
            }

            // Якщо скасовуємо - повертаємо товари на склад
            if (status == "Cancelled" && order.Status != "Cancelled")
            {
                foreach (var orderItem in order.OrderItems ?? new List<OrderItem>())
                {
                    var product = await _productRepository.GetProductByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.Stock += orderItem.Quantity;
                        await _productRepository.UpdateProductAsync(product.Id, product);
                    }
                }
            }

            await _orderRepository.UpdateOrderStatusAsync(orderId, status);
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        // ✅ Скасувати замовлення (користувачем)
        public async Task<bool> CancelOrderAsync(int orderId, int accountId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            // Перевірка що замовлення належить користувачу
            if (order.AccountId != accountId)
            {
                throw new UnauthorizedAccessException("Ви не маєте доступу до цього замовлення");
            }

            // Можна скасувати тільки Pending замовлення
            if (order.Status != "Pending")
            {
                throw new Exception($"Неможливо скасувати замовлення зі статусом '{order.Status}'. Можна скасувати тільки замовлення зі статусом 'Pending'");
            }

            // Повертаємо товари на склад
            foreach (var orderItem in order.OrderItems ?? new List<OrderItem>())
            {
                var product = await _productRepository.GetProductByIdAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.Stock += orderItem.Quantity;
                    await _productRepository.UpdateProductAsync(product.Id, product);
                }
            }

            return await _orderRepository.CancelOrderAsync(orderId);
        }
    }
}