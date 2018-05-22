using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TechChallenge.DomainLogic.Models
{
    public class Order
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public int OrderId { get; set; }

        public string Status { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateShipped { get; set; }

        public List <OrderItem> Items { get; set; }

        public static Order FromJToken(JToken token)
        {
            int orderId = int.Parse(token["orderId"].ToString());
            string status = token["status"].ToString();
            var dateCreated = DateTime.Parse(token["dateCreated"].ToString());
            var shipped = token.SelectToken("dateShipped");
            DateTime? dateShipped = null;
            if (shipped != null)
            {
                dateShipped = DateTime.Parse(shipped.ToString());
            }

            var items = token["items"].Select(OrderItem.FromJToken).ToList();

            return new Order
            {
                OrderId = orderId,
                Status = status,
                DateCreated = dateCreated,
                DateShipped = dateShipped,
                Items = items
            };
        }
    }
}