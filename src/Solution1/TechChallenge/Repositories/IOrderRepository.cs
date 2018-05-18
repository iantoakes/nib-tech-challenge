using TechChallenge.Models;

namespace TechChallenge.Repositories
{
    public interface IOrderRepository
    {
        Order GetOrder(int orderId);

        void AddOrder(Order order);

        void UpdateOrder(Order order);
    }
}
