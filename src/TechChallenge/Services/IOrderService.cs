using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallenge.DomainLogic.Models;
using TechChallenge.Models;

namespace TechChallenge.Services
{
    public interface IOrderService
    {
        Order GetOrder(int orderId);
        void AddOrder(Order order);
        void UpdateOrderStatus(int orderId, string status);
    }
}
