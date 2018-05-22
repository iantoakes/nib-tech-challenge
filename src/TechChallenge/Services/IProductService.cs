using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechChallenge.DomainLogic.Models;
using TechChallenge.Models;

namespace TechChallenge.Services
{
    public interface IProductService
    {
        int DecreaseStockLevel(int productId, int quantity);
        Product GetProduct(int productId);
        List<Product> GetProducts(List<int> productIds);
        void AddProduct(Product product);
        void ReorderProduct(int productId, int quantity);
    }
}
