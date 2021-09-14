#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shops.Entities
{
    public class DeliveryAgent
    {
        private List<Product> _catalog;

        public DeliveryAgent()
        {
            _catalog = new List<Product>();
        }

        public IReadOnlyList<Product> Catalog => _catalog;

        public Product RegisterProduct(string productName)
        {
            Product? catalogProduct = _catalog.Find(catalogProduct => catalogProduct.Name == productName);
            if (catalogProduct is not null)
                return catalogProduct;
            _catalog.Add(new Product(productName));
            return _catalog.Find(catalogProduct => catalogProduct.Name == productName) !;
        }

        public void DeliverProductToShop(Shop shop, SellableProduct product)
        {
            Product? catalogProduct = _catalog.Find(catalogProduct => catalogProduct == product.CountableProduct.Product);
            if (catalogProduct is null)
                throw new Exception("Product is not registered");
            shop.GiveProduct((SellableProduct)product.Clone());
        }

        public Product GetProduct(string productName)
        {
            Product? product = _catalog.Find(product => product.Name == productName);
            if (product is null)
                throw new Exception("Product is not registered");
            return product;
        }
    }
}