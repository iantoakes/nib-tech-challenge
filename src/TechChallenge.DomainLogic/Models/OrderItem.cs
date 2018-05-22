using Newtonsoft.Json.Linq;

namespace TechChallenge.DomainLogic.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal CostPerItem { get; set; }

        public static OrderItem FromJToken(JToken token)
        {
            int orderId = int.Parse(token["orderId"].ToString());
            int productId = int.Parse(token["productId"].ToString());
            int quantity = int.Parse(token["quantity"].ToString());
            decimal costPerItem = decimal.Parse(token["costPerItem"].ToString());

            return new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                CostPerItem = costPerItem
            };
        }
    }
}