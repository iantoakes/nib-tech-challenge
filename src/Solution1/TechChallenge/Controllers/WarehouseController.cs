using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NLog;
using TechChallenge.Models;
using TechChallenge.Services;

namespace TechChallenge.Controllers
{
    [RoutePrefix("api/v1/warehouse")]
    public class WarehouseController : ApiController
    {
        private readonly IWarehouseService _warehouseService;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
                    var productIds = order.Items.Select(i => i.ProductId).ToList();
                    var products = _warehouseService.GetProducts(productIds);

                    if (products.Any(p => p.QuantityOnHand < order.Items.First(i => i.ProductId == p.ProductId).Quantity))
                    {
                        rejectedOrders.Add(orderId);
                        _warehouseService.UpdateOrderStatus(order.OrderId, FulfillmentStatus.Error);
                        continue;
                    }

                    foreach (var item in order.Items)
                    {
                        _warehouseService.DecreaseStockLevel(item.ProductId, item.Quantity);
                        var product = _warehouseService.GetProduct(item.ProductId);
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
                Logger.Error(ex, ex.Message);
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
