using System.Collections.Generic;
using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Services
{
    public interface IWarehouseService
    {
        int DecreaseStockLevel(int productId, int quantity);

        Product GetProduct(int productId);

        List<Product> GetProducts(List<int> productIds);

        void ReorderProduct(int productId, int quantity);

        void UpdateOrderStatus(int orderId, string status);

        void AddProduct(Product product);

        Order GetOrder(int orderId);

        void AddOrder(Order order);

        List<int> FulfillOrder(List<int> orderIds);
    }
}
