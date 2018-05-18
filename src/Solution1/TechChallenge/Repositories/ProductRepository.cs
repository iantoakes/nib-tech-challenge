using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using TechChallenge.Models;

namespace TechChallenge.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Dictionary<int, Product> _productDataStore;

        public ProductRepository(string dataFileName)
        {
            string json = File.ReadAllText(Path.Combine(HttpRuntime.AppDomainAppPath, dataFileName));

            var jobj = JObject.Parse(json);
            var products = jobj["products"].Select(Product.FromJToken).ToList();
            _productDataStore = products.ToDictionary(p => p.ProductId);
        }

        public Product GetProduct(int productId)
        {
            Product product;
            _productDataStore.TryGetValue(productId, out product);
            return product;
        }

        public List<Product> GetProducts(List<int> productIds)
        {
            return productIds.Select(GetProduct).Where(p => p != null).ToList();
        }

        public void AddProduct(Product product)
        {
            _productDataStore[product.ProductId] = product;
        }
    }
}