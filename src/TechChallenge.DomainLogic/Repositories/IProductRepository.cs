using System.Collections.Generic;
using TechChallenge.DomainLogic.Models;

namespace TechChallenge.DomainLogic.Repositories
{
    public interface IProductRepository
    {
        Product GetProduct(int productId);

        List<Product> GetProducts(List<int> productIds);

        void AddProduct(Product product);
    }
}