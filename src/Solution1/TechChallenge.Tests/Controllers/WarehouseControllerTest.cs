using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using TechChallenge.Controllers;
using TechChallenge.Models;
using TechChallenge.Services;

namespace TechChallenge.Tests.Controllers
{
    [TestClass]
    public class WarehouseControllerTest
    {
        [TestMethod]
        public void FulfilmentRejectsOrderWhenInsufficientProductQuantity()
        {
            int productId = 1;
            int orderId = 1;

            var product = new Product { ProductId = productId, QuantityOnHand = 0 };

            var order = new Order
            {
                OrderId = orderId,
                Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 50 } }
            };

            var products = new List<Product> {product};
            var productIds = new List<int> {productId};

            var mockService = new Mock<IWarehouseService>();
            mockService.Setup(s => s.GetProduct(productId)).Returns(product);
            mockService.Setup(s => s.GetProducts(productIds)).Returns(products);
            mockService.Setup(s => s.GetOrder(orderId)).Returns(order);

            var controller = BuildControlller(mockService);

            var result = controller.Fulfilment(new FulfilmentRequest { OrderIds = new List<int> { 1 } });
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<object>));

            var jobj = JObject.Parse(result.ExecuteAsync(new CancellationToken()).Result.Content.ReadAsStringAsync().Result);
            var response = jobj.ToObject<FulfillmentResponse>();

            Assert.IsTrue(response.Unfulfillable.Count == 1);
            Assert.IsTrue(response.Unfulfillable[0] == 1);
        }

        [TestMethod]
        public void FulfilmentPlacesOrderWhenOneIsRejected()
        {
            int productId1 = 1;
            int productId2 = 2;

            int orderId1 = 1;
            int orderId2 = 2;

            var product1 = new Product { ProductId = productId1, QuantityOnHand = 100 };
            var product2 = new Product { ProductId = productId2, QuantityOnHand = 0 };

            var order1 = new Order
            {
                OrderId = orderId1,
                Items = new List<OrderItem> { new OrderItem { ProductId = productId1, Quantity = 50 } }
            };

            var order2 = new Order
            {
                OrderId = orderId2,
                Items = new List<OrderItem> { new OrderItem { ProductId = productId2, Quantity = 50 } }
            };

            var productIds1 = new List<int> { productId1 };
            var productIds2 = new List<int> { productId2 };
            var products1 = new List<Product> {product1};
            var products2 = new List<Product> {product2};

            var mockService = new Mock<IWarehouseService>();
            mockService.Setup(s => s.GetProduct(productId1)).Returns(product1);
            mockService.Setup(s => s.GetProduct(productId2)).Returns(product2);

            mockService.Setup(s => s.GetProducts(productIds1)).Returns(products1);
            mockService.Setup(s => s.GetProducts(productIds2)).Returns(products2);

            mockService.Setup(s => s.GetOrder(orderId1)).Returns(order1);
            mockService.Setup(s => s.GetOrder(orderId2)).Returns(order2);

            var controller = BuildControlller(mockService);
            controller.Fulfilment(new FulfilmentRequest { OrderIds = new List<int> { orderId1, orderId2 } });

            mockService.Verify(s => s.UpdateOrderStatus(orderId1, FulfillmentStatus.Success), Times.Once, $"Order status not updated for order {orderId1}");
        }

        [TestMethod]
        public void FulfilmentReordersProductWhenQuantityFallsBelowThreshold()
        {
            int productId = 1;
            int orderId = 1;

            var product = new Product { ProductId = productId, QuantityOnHand = 90, ReorderThreshold = 50 };

            var order = new Order
            {
                OrderId = orderId,
                Items = new List<OrderItem> { new OrderItem { ProductId = productId, Quantity = 50 } }
            };

            var productIds = new List<int> { productId };
            var products = new List<Product> { product };

            var mockService = new Mock<IWarehouseService>();
            mockService.Setup(s => s.GetProduct(productId)).Returns(product);
            mockService.Setup(s => s.GetProducts(productIds)).Returns(products);

            mockService.Setup(s => s.GetOrder(It.IsAny<int>())).Returns((int id) => order);
            mockService.Setup(s => s.DecreaseStockLevel(productId, It.IsAny<int>())).Returns((int id, int quantity) =>
            {
                product.QuantityOnHand -= quantity;
                return product.QuantityOnHand;
            });

            var controller = BuildControlller(mockService);
            controller.Fulfilment(new FulfilmentRequest { OrderIds = new List<int> { orderId } });

            mockService.Verify(s => s.ReorderProduct(productId, product.ReorderAmount), Times.Once, "Failed to reorder product");
        }

        [TestMethod]
        public void FulfilmentUpdatesOrderStatusForSucessfulOrder()
        {
            int productId = 1;
            int orderId = 1;

            var product = new Product { ProductId = productId, QuantityOnHand = 90, ReorderThreshold = 50 };

            var order = new Order
            {
                OrderId = orderId,
                Items = new List<OrderItem> { new OrderItem { ProductId = productId, Quantity = 10 } }
            };

            var productIds = new List<int> { productId };
            var products = new List<Product> { product };

            var mockService = new Mock<IWarehouseService>();
            mockService.Setup(s => s.GetProduct(productId)).Returns(product);
            mockService.Setup(s => s.GetProducts(productIds)).Returns(products);

            mockService.Setup(s => s.GetOrder(orderId)).Returns(order);

            var controller = BuildControlller(mockService);
            controller.Fulfilment(new FulfilmentRequest { OrderIds = new List<int> { orderId } });

            mockService.Verify(s => s.UpdateOrderStatus(orderId, FulfillmentStatus.Success), Times.Once, "Failed to update order status");
        }

        [TestMethod]
        public void FulfilmentUpdatesOrderStatusForUnsucessfulOrder()
        {
            int productId = 1;
            int orderId = 1;

            var product = new Product { ProductId = productId, QuantityOnHand = 0, ReorderThreshold = 50 };

            var order = new Order
            {
                OrderId = orderId,
                Items = new List<OrderItem> { new OrderItem { ProductId = productId, Quantity = 10 } }
            };

            var productIds = new List<int> { productId };
            var products = new List<Product> { product };

            var mockService = new Mock<IWarehouseService>();
            mockService.Setup(s => s.GetProduct(productId)).Returns(product);
            mockService.Setup(s => s.GetProducts(productIds)).Returns(products);

            mockService.Setup(s => s.GetOrder(orderId)).Returns(order);

            var controller = BuildControlller(mockService);
            controller.Fulfilment(new FulfilmentRequest { OrderIds = new List<int> { orderId } });

            mockService.Verify(s => s.UpdateOrderStatus(orderId, FulfillmentStatus.Error), Times.Once, "Failed to update order status");
        }

        private static WarehouseController BuildControlller(Mock<IWarehouseService> mockService)
        {
            var controller = new WarehouseController(mockService.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }
    }
}
