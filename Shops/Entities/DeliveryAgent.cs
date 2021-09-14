#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shops.Entities
{
    public class DeliveryAgent
    {
        private List<CatalogProduct> _catalog;

        public DeliveryAgent()
        {
            _catalog = new List<CatalogProduct>();
        }

        public IReadOnlyList<CatalogProduct> Catalog => _catalog;

        public void RegisterProduct(string productName)
        {
            CatalogProduct? catalogProduct = _catalog.Find(catalogProduct => catalogProduct.Product.Name == productName);
            if (catalogProduct is null)
                _catalog.Add(new CatalogProduct(new Product(productName)));
        }

        public void DeliverProductToShop(Shop shop, SellableProduct product)
        {
            CatalogProduct? catalogProduct = _catalog.Find(catalogProduct => catalogProduct.Product == product.CountableProduct.Product);
            if (catalogProduct is null)
                throw new Exception("Product is not registered");
            shop.GiveProduct((SellableProduct)product.Clone());
            catalogProduct.Shops.Add(shop);
        }

        public Product GetProduct(string productName)
        {
            CatalogProduct? product = _catalog.Find(product => product.Product.Name == productName);
            if (product is null)
                throw new Exception("Product is not registered");
            return product.Product;
        }

        public IReadOnlyList<Shop> GetShops(Product product)
        {
            CatalogProduct? catalogProduct = _catalog.Find(catalogProduct => catalogProduct.Product.Equals(product));
            if (product is null)
                throw new Exception("Product is not registered");
            return catalogProduct!.Shops;
        }
    }
}