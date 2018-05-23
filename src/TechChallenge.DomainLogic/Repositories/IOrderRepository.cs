using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Repositories
{
    public interface IOrderRepository
    {
        Order GetOrder(int orderId);

        void AddOrder(Order order);

        void UpdateOrder(Order order);
    }
}
