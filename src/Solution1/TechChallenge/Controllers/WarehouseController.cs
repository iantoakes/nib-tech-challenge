using System;
using System.Collections.Generic;
using System.Web.Http;
using TechChallenge.Models;
using TechChallenge.Services;

namespace TechChallenge.Controllers
{
    public class WarehouseController : ApiController
    {
        private readonly IWarehouseService _warehouseService;

        public WarehouseController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpPost, Route("fulfilment")]
        public IHttpActionResult Fulfilment(FulfilmentRequest fulfilmentRequest)
        {
            try
            {
                var rejectedOrders = new List<int>();
                foreach (int orderId in fulfilmentRequest.OrderIds)
                {
                    var order = _warehouseService.GetOrder(orderId);
                    foreach (var item in order.Items)
                    {
                        var product = _warehouseService.GetProduct(item.ProductId);
                        if (product.QuantityOnHand < item.Quantity)
                        {
                            rejectedOrders.Add(orderId);
                            _warehouseService.UpdateOrderStatus(order.OrderId, FulfillmentStatus.Error);
                            break;
                        }

                        _warehouseService.DecreaseStockLevel(item.ProductId, item.Quantity);
                        product = _warehouseService.GetProduct(item.ProductId);
                        if (product.QuantityOnHand < product.ReorderThreshold)
                        {
                            _warehouseService.ReorderProduct(product.ProductId, product.ReorderAmount);
                        }
                        _warehouseService.UpdateOrderStatus(order.OrderId, FulfillmentStatus.Success);
                    }
                }

                var response = rejectedOrders.Count > 0
                    ? new FulfillmentResponse { Unfulfillable = rejectedOrders }
                    : new object();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("product")]
        public IHttpActionResult PostProduct(Product product)
        {
            _warehouseService.AddProduct(product);
            return Ok();
        }

        [HttpGet, Route("product/{productId}")]
        public IHttpActionResult GetProduct(int productId)
        {
            try
            {
                return Ok(_warehouseService.GetProduct(productId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("order")]
        public IHttpActionResult PostOrder(Order order)
        {
            _warehouseService.AddOrder(order);
            return Ok();
        }

        [HttpGet, Route("order/{orderId}")]
        public IHttpActionResult GetOrder(int orderId)
        {
            try
            {
                return Ok(_warehouseService.GetOrder(orderId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("test")]
        public IHttpActionResult Test()
        {
            return Ok("Hello World");
        }
    }
}
