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
            _shops.Add(new Shop(_maxId, shopName, address));
            return Shops.FirstOrDefault(shop => shop.Id == _maxId) !;
        }

        public void AddProduct(Shop shop, SellableProduct product)
        {
            if (!Catalog.Contains(product))
                throw new ShopServiceException("There is no such product");
            _deliveryAgent.DeliverProductToShop(GetShopFromShops(shop), product);
        }

        public void ChangePrice(Shop shop, SellableProduct product, uint newPrice)
        {
            Shop localShop = GetShopFromShops(shop);
            if (!localShop.Products.Contains(product))
                throw new ShopServiceException("There is no such product");
            localShop.Products.FirstOrDefault(shopProduct => shopProduct.Name == product.Name) !.Cost = newPrice;
        }

        public uint FindCheapestPrice(string productName, uint count)
        {
            if (Catalog.All(product => product.Name != productName))
                throw new Exception("There is no such product");
            List<Shop> found = Shops.Where(shop => shop.CheckAvailability(new Product(productName, count)))
                .OrderBy(shop => shop.Products.FirstOrDefault(product => product.Name == productName) !.Cost).ToList();
            return found[0].Id;
        }

        public void Buy(Buyer buyer, Shop shop, Product product)
        {
            if (!Catalog.Contains(product))
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