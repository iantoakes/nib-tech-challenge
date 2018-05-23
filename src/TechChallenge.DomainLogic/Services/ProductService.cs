using System;
using System.Collections.Generic;
using TechChallenge.DomainLogic.Models;
using TechChallenge.DomainLogic.Repositories;

namespace TechChallenge.DomainLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public int DecreaseStockLevel(int productId, int quantity)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                throw new Exception($"Unable to decrease stock level for {productId} as it doesn't exist");
            }

            if (product.QuantityOnHand > quantity)
            {
                product.QuantityOnHand -= quantity;
            }
            else
            {
                product.QuantityOnHand = 0;
            }

            return product.QuantityOnHand;
        }

        public Product GetProduct(int productId)
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null) throw new Exception($"Unable to get product for {productId} as it doesn't exist");

            return product;
        }

        public List<Product> GetProducts(List<int> productIds)
        {
            if (productIds == null) throw new ArgumentNullException(nameof(productIds));
            if (productIds.Count == 0) throw new ArgumentException("Expecting a list of product ids", nameof(productIds));

            return _productRepository.GetProducts(productIds);
        }

        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _productRepository.AddProduct(product);
        }

        public void ReorderProduct(int productId, int quantity)
        {
            if (productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

            var product = _productRepository.GetProduct(productId);
            if (product == null) throw new ArgumentOutOfRangeException(nameof(productId), "Specified product doesn't exist");

            // calls legacy system to reorder product
        }

    }
}