using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Services
{
    public interface IOrderService
    {
        Order GetOrder(int orderId);
        void AddOrder(Order order);
        void UpdateOrderStatus(int orderId, string status);
    }
}
