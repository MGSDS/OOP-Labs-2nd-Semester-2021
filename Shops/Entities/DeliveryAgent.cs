using System;
using System.Collections.Generic;
using System.Linq;

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

        public CatalogProduct RegisterProduct(string productName)
        {
            if (_catalog.All(catalogProduct => catalogProduct.Name != productName))
                _catalog.Add(new CatalogProduct(productName));
            return _catalog.FirstOrDefault(product => product.Name == productName);
        }

        public void DeliverProductToShop(Shop shop, SellableProduct product)
        {
            if (_catalog.All(catalogProduct => catalogProduct.Name != product.Name))
                throw new Exception("Product is not registered");
            shop.GiveProduct((SellableProduct)product.Clone());
        }
    }
}