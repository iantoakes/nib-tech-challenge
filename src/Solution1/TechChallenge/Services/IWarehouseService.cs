using TechChallenge.Models;

namespace TechChallenge.Services
{
    public interface IWarehouseService
    {
        int DecreaseStockLevel(int productId, int quantity);

        Product GetProduct(int productId);

        void ReorderProduct(int productId, int quantity);

        void UpdateOrderStatus(int orderId, string status);
        void AddProduct(Product product);

        Order GetOrder(int orderId);

        void AddOrder(Order order);
    }
}
