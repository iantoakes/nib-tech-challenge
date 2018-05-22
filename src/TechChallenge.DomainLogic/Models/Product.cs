using Newtonsoft.Json.Linq;

namespace TechChallenge.DomainLogic.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Description { get; set; }

        public int QuantityOnHand { get; set; }

        public int ReorderThreshold { get; set; }

        public int ReorderAmount { get; set; }

        public int DeliveryLeadTime { get; set; }

        public static Product FromJToken(JToken token)
        {
            int productId = int.Parse(token["productId"].ToString());
            int quantityOnHand = int.Parse(token["quantityOnHand"].ToString());
            int reorderThreshold = int.Parse(token["reorderThreshold"].ToString());
            int reorderAmount = int.Parse(token["reorderAmount"].ToString());
            int deliveryLeadtime = int.Parse(token["deliveryLeadTime"].ToString());

            return new Product
            {
                ProductId = productId,
                QuantityOnHand = quantityOnHand,
                ReorderThreshold = reorderThreshold,
                ReorderAmount = reorderAmount,
                DeliveryLeadTime = deliveryLeadtime
            };
        }
    }
}