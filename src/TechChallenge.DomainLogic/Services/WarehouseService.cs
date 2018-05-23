using System;
using System.Collections.Generic;
using System.Linq;
using TechChallenge.DomainLogic.Models;
using TechChallenge.DomainLogic.Repositories;

namespace TechChallenge.DomainLogic.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public WarehouseService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public int DecreaseStockLevel(int productId, int quantity)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                throw new Exception($"Unable to decrease stock level for {productId} as it doesn't exist");
            }

            if (product.QuantityOnHand > quantity)
            {
                product.QuantityOnHand -= quantity;
            }
            else
            {
                product.QuantityOnHand = 0;
            }

            return product.QuantityOnHand;
        }

        public Product GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null) throw new Exception($"Unable to get product for {productId} as it doesn't exist");

            return product;
        }

        public List<Product> GetProducts(List<int> productIds)
        {
            if (productIds == null) throw new ArgumentNullException(nameof(productIds));
            if(productIds.Count == 0) throw new ArgumentException("Expecting a list of product ids", nameof(productIds));

            return _productRepository.GetProducts(productIds);
        }

        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _productRepository.AddProduct(product);
        }

        public void ReorderProduct(int productId, int quantity)
        {
            if(productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
            if(quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var product = _productRepository.GetProduct(productId);
            if(product == null) throw new ArgumentOutOfRangeException(nameof(productId), "Specified product doesn't exist");
            
            // calls legacy system to reorder product
        }

        public Order GetOrder(int orderId)
        {
            var order = _orderRepository.GetOrder(orderId);
            if(order == null) throw new ArgumentOutOfRangeException($"Unable to get order for {orderId} as it doesn't exist");

            return order;
        }

        public void AddOrder(Order order)
        {
            if(order == null) throw new ArgumentNullException(nameof(order));

            _orderRepository.AddOrder(order);
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            if(string.IsNullOrWhiteSpace(status) || FulfillmentStatus.ValidStatuses.Contains(status) == false) throw new ArgumentException("Invalid status", nameof(status));

            var order = _orderRepository.GetOrder(orderId);
            if(order == null) throw new ArgumentException("Unable to find matching order", nameof(orderId));

            order.Status = status;
            _orderRepository.UpdateOrder(order);
        }

        public List<int> FulfillOrder(List<int> orderIds)
        {
            var rejectedOrders = new List<int>();
            foreach (int orderId in orderIds)
            {
                var order = GetOrder(orderId);
                var productIds = order.Items.Select(i => i.ProductId).ToList();
                var products = GetProducts(productIds);

                if (products.Any(p => p.QuantityOnHand < order.Items.First(i => i.ProductId == p.ProductId).Quantity))
                {
                    rejectedOrders.Add(orderId);
                    UpdateOrderStatus(order.OrderId, FulfillmentStatus.Error);
                    continue;
                }

                foreach (var item in order.Items)
                {
                    DecreaseStockLevel(item.ProductId, item.Quantity);
                    var product = GetProduct(item.ProductId);
                    if (product.QuantityOnHand < product.ReorderThreshold)
                    {
                        ReorderProduct(product.ProductId, product.ReorderAmount);
                    }
                    UpdateOrderStatus(order.OrderId, FulfillmentStatus.Success);
                }
            }

            return rejectedOrders;
        }
    }
}