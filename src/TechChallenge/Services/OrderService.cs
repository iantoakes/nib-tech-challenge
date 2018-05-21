using System;
using TechChallenge.Models;
using TechChallenge.Repositories;

namespace TechChallenge.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public Order GetOrder(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if (order == null) throw new ArgumentOutOfRangeException($"Unable to get order for {orderId} as it doesn't exist");

            return order;
        }

        public void AddOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            _orderRepository.AddOrder(order);
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            if (string.IsNullOrWhiteSpace(status) || FulfillmentStatus.ValidStatuses.Contains(status) == false) throw new ArgumentException("Invalid status", nameof(status));

            var order = _orderRepository.GetOrder(orderId);
            if (order == null) throw new ArgumentException("Unable to find matching order", nameof(orderId));

            order.Status = status;
            _orderRepository.UpdateOrder(order);
        }
    }
}