#nullable enable
using System;
using System.Collections.Generic;
using Shops.Interfaces;
using Shops.Tools;

namespace Shops.Entities
{
    public class DeliveryAgent : ICloneable<DeliveryAgent>
    {
        private List<Product> _catalog;

        public DeliveryAgent()
        {
            _catalog = new List<Product>();
        }

        public DeliveryAgent(List<Product> catalog)
        {
            _catalog = new List<Product>(catalog);
        }

        public IReadOnlyList<Product> Catalog => _catalog;

        public Product RegisterProduct(string productName)
        {
            Product? catalogProduct = _catalog.Find(catalogProduct => catalogProduct.Name == productName);
            if (catalogProduct is not null)
                return catalogProduct;
            catalogProduct = new Product(productName);
            _catalog.Add(catalogProduct);
            return catalogProduct;
        }

        public void DeliverProductToShop(Shop shop, SellableProduct product)
        {
            Product? catalogProduct = _catalog.Find(catalogProduct => catalogProduct == product.CountableProduct.Product);
            if (catalogProduct is null)
                throw new ShopServiceException("Product is not registered");
            shop.GiveProduct(product.Clone());
        }

        public Product GetProduct(string productName)
        {
            Product? product = _catalog.Find(product => product.Name == productName);
            if (product is null)
                throw new ShopServiceException("Product is not registered");
            return product;
        }

        public DeliveryAgent Clone()
        {
            return new DeliveryAgent(new List<Product>(_catalog));
        }
    }
}