using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechChallenge.IntegrationTests.Facades;
using TechChallenge.Models;
using TechTalk.SpecFlow;

namespace TechChallenge.IntegrationTests
{
    [Binding]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FulfillmentSteps
    {
        private Order _order;
        private Product _product;
        private readonly WarehouseFacade _warehouseFacade;
        private FulfillmentResponse _fulfillmentResponse;

        public FulfillmentSteps()
        {
            _warehouseFacade = new WarehouseFacade();
        }

        [Given(@"I have an order awaiting fulfilment")]
        public void GivenIHaveAnOrderAwaitingFulfilment()
        {
            _order = new Order
            {
                OrderId = 1,
                Status = FulfillmentStatus.Pending,
                DateCreated = DateTime.Now
            };
        }

        [Given(@"the order has ""(.*)"" included")]
        public void GivenTheOrderHasIncluded(string product)
        {
            _product = new Product { ProductId = 1, Description = product };
            _order.Items.Add(new OrderItem { OrderId = _order.OrderId, ProductId = _product.ProductId, Quantity = 10, CostPerItem = 10.50m });
        }

        [Given(@"stock is available to fulfil that order")]
        public void GivenStockIsAvailableToFulfilThatOrder()
        {
            _product.QuantityOnHand = 100;
        }

        [When(@"I submit the fulfillment")]
        public void WhenISubmitTheFulfillment()
        {
            SetupData();

            _fulfillmentResponse = _warehouseFacade.FulfillOrder(_order.OrderId);
        }

        [Then(@"I expect the order to be processed")]
        public void ThenIExpectTheOrderToBeProcessed()
        {
            Assert.IsNull(_fulfillmentResponse.Unfulfillable, $"Fulfillment repsonse contains failed order ids");
        }

        [Then(@"I expect that the order status is ""(.*)""")]
        public void ThenIExpectThatTheOrderStatusIs(string orderStatus)
        {
            var order = _warehouseFacade.GetOrder(_order.OrderId);

            Assert.AreEqual(orderStatus, order.Status, $"Expected order status of {orderStatus}, but was {order.Status}");
        }

        [Then(@"I expect that product quantity in the Legacy system is updated")]
        public void ThenIExpectThatProductQuantityInTheLegacySystemIsUpdated()
        {
            var product = _warehouseFacade.GetProduct(_product.ProductId);
            int expected = _product.QuantityOnHand - _order.Items[0].Quantity;

            Assert.AreEqual(expected, product.QuantityOnHand, $"Expected {expected} products on hand, but was {product.QuantityOnHand}");
        }

        private void SetupData()
        {
            _warehouseFacade.AddProduct(_product);
            _warehouseFacade.AddOrder(_order);
        }
    }
}
