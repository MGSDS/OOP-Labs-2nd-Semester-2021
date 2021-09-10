#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Services
{
    public class ShopService
    {
        private DeliveryAgent _deliveryAgent;
        private List<Shop> _shops;
        private uint _maxId;

        public ShopService()
        {
            _maxId = 0;
            _shops = new List<Shop>();
            _deliveryAgent = new DeliveryAgent();
        }

        public IReadOnlyList<Shop> Shops => _shops;
        public IReadOnlyList<CatalogProduct> Catalog => _deliveryAgent.Catalog;

        public CatalogProduct RegisterProduct(string productName)
        {
            _deliveryAgent.RegisterProduct(productName);
            return Catalog.FirstOrDefault(product => product.Name == productName) !;
        }

        public Shop RegisterShop(string shopName, string address)
        {
            _shops.Add(new Shop(_maxId++, shopName, address));
            return Shops.FirstOrDefault(shop => shop.Id == _maxId - 1) !;
        }

        public Product AddProduct(Shop shop, SellableProduct product)
        {
            if (Catalog.All(catalogProduct => catalogProduct.Name != product.Name))
                throw new ShopServiceException("There is no such product registered");
            _deliveryAgent.DeliverProductToShop(GetShopFromShops(shop), product);
            return GetShopFromShops(shop).Products.FirstOrDefault(localProduct => localProduct.Name == product.Name) !;
        }

        public void ChangePrice(Shop shop, IProduct product, uint newPrice)
        {
            Shop localShop = GetShopFromShops(shop);
            if (localShop.Products.All(localProduct => product.Name != localProduct.Name))
                throw new ShopServiceException("There is no such product");
            localShop.Products.FirstOrDefault(shopProduct => shopProduct.Name == product.Name) !.Price = newPrice;
        }

        public Shop FindCheapestPrice(Product product)
        {
            if (Catalog.All(localProduct => product.Name != localProduct.Name))
                throw new ShopServiceException("There is no such product");
            List<Shop> found = Shops.Where(shop => shop.CheckAvailability(product))
                .OrderBy(shop => shop.Products.FirstOrDefault(localProduct => product.Name == localProduct.Name) !.Price).ToList();
            if (!found.Any())
                throw new ShopServiceException("There is no not enough product in any shop");
            return found[0];
        }

        public void Buy(Buyer buyer, Shop shop, Product product)
        {
            if (Catalog.All(localProduct => product.Name != localProduct.Name))
                throw new ShopServiceException("There is no such product");
            GetShopFromShops(shop).Sell(buyer, product);
        }

        private Shop GetShopFromShops(Shop shop)
        {
            if (!_shops.Contains(shop))
                throw new ShopServiceException("There is no such shop");
            return _shops.FirstOrDefault(listedShop => listedShop.Id == shop.Id) !;
        }
    }
}