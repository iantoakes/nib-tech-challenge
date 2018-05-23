using System.Collections.Generic;
using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Services
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
