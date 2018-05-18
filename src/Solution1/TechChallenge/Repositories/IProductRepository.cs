using System.Collections.Generic;
using TechChallenge.Models;

namespace TechChallenge.Repositories
{
    public interface IProductRepository
    {
        Product GetProduct(int productId);

        List<Product> GetProducts(List<int> productIds);

        void AddProduct(Product product);
    }
}