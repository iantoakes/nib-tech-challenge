using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using TechChallenge.Models;

namespace TechChallenge.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly Dictionary<int, Product> _productDataStore;
        private readonly Dictionary<int, Order> _orderDataStore;

        public WarehouseService()
        {
            string json = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, @"Data\data.json"));

            var jobj = JObject.Parse(json);
            var products = jobj["products"].Select(Product.FromJToken).ToList();
            _productDataStore = products.ToDictionary(p => p.ProductId);

            var orders = jobj["orders"].Select(Order.FromJToken).ToList();
            _orderDataStore = orders.ToDictionary(o => o.OrderId);
        }

        public int DecreaseStockLevel(int productId, int quantity)
        {
            Product product;
            if (!_productDataStore.TryGetValue(productId, out product))
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
            Product product;
            if (!_productDataStore.TryGetValue(productId, out product))
            {
                throw new Exception($"Unable to get product for {productId} as it doesn't exist");
            }

            return product;
        }

        public List<Product> GetProducts(List<int> productIds)
        {
            return productIds.Select(GetProduct).ToList();
        }

        public void AddProduct(Product product)
        {
            _productDataStore[product.ProductId] = product;
        }

        public void ReorderProduct(int productId, int quantity)
        {
            // calls legacy system to reorder product
        }

        public Order GetOrder(int orderId)
        {
            Order order;
            if (_orderDataStore.TryGetValue(orderId, out order) == false)
            {
                throw new Exception($"Unable to get order for {orderId} as it doesn't exist");
            }
            return _orderDataStore[orderId];
        }

        public void AddOrder(Order order)
        {
            _orderDataStore[order.OrderId] = order;
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            Order order;
            if (_orderDataStore.TryGetValue(orderId, out order) == false)
            {
                throw new Exception($"Unable to get update order status to {status} for {orderId} as it doesn't exist");
            }
            _orderDataStore[orderId].Status = status;
        }
    }
}