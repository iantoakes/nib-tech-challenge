using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac.Features.AttributeFilters;
using Newtonsoft.Json.Linq;
using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Dictionary<int, Order> _orderDataStore;

        public OrderRepository([KeyFilter("DataFilePath")]string dataFileName)
        {
            //string json = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, dataFileName));
            string json = File.ReadAllText(dataFileName);
            //string json = "";

            var jobj = JObject.Parse(json);

            var orders = jobj["orders"].Select(Order.FromJToken).ToList();
            _orderDataStore = orders.ToDictionary(o => o.OrderId);
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

        public void UpdateOrder(Order order)
        {
            _orderDataStore[order.OrderId] = order;
        }
    }
}