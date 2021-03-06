﻿using System;
using System.Web.Http;
using NLog;
using TechChallenge.DomainLogic.Models;
using TechChallenge.DomainLogic.Services;
using TechChallenge.Models;

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
                if (fulfilmentRequest?.OrderIds == null)
                {
                    return BadRequest("Request body should contain an array of orderIds");
                }

                var rejectedOrders = _warehouseService.FulfillOrder(fulfilmentRequest.OrderIds);

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
