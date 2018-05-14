using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using TechChallenge.Models;

namespace TechChallenge.IntegrationTests.Facades
{
    public class WarehouseFacade
    {
        const string TechChallengeUrl = "http://localhost/TechChallenge";

        public FulfillmentResponse FulfillOrder(int orderId)
        {
            var request = new RestRequest("fulfilment", Method.POST) { RequestFormat = DataFormat.Json };

            var body = new { OrderIds = new[] { orderId } };
            request.AddBody(body);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on fulfillment");

            //return new FulfillmentResponse();

            var fresponse = JsonConvert.DeserializeObject<FulfillmentResponse>(response.Content);
            ////var fresponse = jobj.ToObject<FulfillmentResponse>();
            return fresponse;
        }

        public Order GetOrder(int orderId)
        {
            var request = new RestRequest("order/{orderId}", Method.GET) { RequestFormat = DataFormat.Json };
            request.AddUrlSegment("orderId", orderId);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on order/{orderId}");

            var order = JsonConvert.DeserializeObject<Order>(response.Content);
            return order;
        }

        public Product GetProduct(RestRequest request, int productId)
        {
            request.AddUrlSegment("productId", productId);

            var client = new RestClient(TechChallengeUrl);
            var response = client.Execute(request);

            Assert.IsTrue(response.IsSuccessful, $"StatusCode: {response.StatusCode} on product/{productId}");

            var product = JsonConvert.DeserializeObject<Product>(response.Content);
            return product;
        }

        public void AddOrder(Order order)
        {
            var client = new RestClient("http://localhost/TechChallenge");
            var request = new RestRequest("order", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(order);
            var response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful, $"Error adding order statusCode: {response.StatusCode}");
        }

        public void AddProduct(Product product)
        {
            var client = new RestClient("http://localhost/TechChallenge");
            var request = new RestRequest("product", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(product);
            var response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful, $"Error adding product statusCode: {response.StatusCode}");
        }
    }
}
