using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using TechChallenge.DomainLogic.Models;
using TechChallenge.Models;

namespace TechChallenge.IntegrationTests.Facades
{
    public class WarehouseFacade
    {
        private const string TechChallengeUrl = "http://localhost/TechChallenge";
        private const string RoutePrefix = "/api/v1/warehouse";

        public FulfillmentResponse FulfillOrder(int orderId)
        {
            var request = new RestRequest($"{RoutePrefix}/fulfilment", Method.POST) { RequestFormat = DataFormat.Json };

            var body = new { OrderIds = new[] { orderId } };
            request.AddBody(body);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on fulfillment");

            var fresponse = JsonConvert.DeserializeObject<FulfillmentResponse>(response.Content);
            return fresponse;
        }

        public Order GetOrder(int orderId)
        {
            var request = new RestRequest($"{RoutePrefix}/order/{orderId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddUrlSegment("orderId", orderId);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on order/{orderId}");

            var order = JsonConvert.DeserializeObject<Order>(response.Content);
            return order;
        }

        public void AddOrder(Order order)
        {
            var request = new RestRequest($"{RoutePrefix}/order", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(order);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful, $"Error adding order statusCode: {response.StatusCode}");
        }

        public Product GetProduct(int productId)
        {
            var request = new RestRequest($"{RoutePrefix}/product/{productId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddUrlSegment("productId", productId);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on product/{productId}");

            var product = JsonConvert.DeserializeObject<Product>(response.Content);
            return product;
        }

        public void AddProduct(Product product)
        {
            var request = new RestRequest($"{RoutePrefix}/product", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(product);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful, $"Error adding product statusCode: {response.StatusCode}");
        }
    }
}
