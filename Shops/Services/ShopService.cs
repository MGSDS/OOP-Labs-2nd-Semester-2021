#nullable enable
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

        public void RegisterProduct(string productName)
        {
            _deliveryAgent.RegisterProduct(productName);
        }

        public uint RegisterShop(string shopName, string address)
        {
            _shops.Add(new Shop(_maxId++, shopName, address));
            return _maxId - 1;
        }

        public void AddProduct(uint shopId, string productName, uint count, uint price)
        {
            Product product = _deliveryAgent.GetProduct(productName);
            _deliveryAgent.DeliverProductToShop(GetShopById(shopId), new SellableProduct(new CountableProduct(product, count), price));
        }

        public void ChangePrice(uint shopId, string productName, uint newPrice)
        {
            Shop foundShop = GetShopById(shopId);
            Product foundProduct = _deliveryAgent.GetProduct(productName);
            foundShop.ChangePrice(foundProduct, newPrice);
        }

        public uint FindCheapestPriceShopId(string productName, uint count)
        {
            Product product = _deliveryAgent.GetProduct(productName);
            List<Shop> found = _deliveryAgent.GetShops(product)
                .OrderBy(shop => shop.Products.First(sellableProduct => sellableProduct.CountableProduct.Product.Equals(product)).Price).ToList();
            if (!found.Any())
                throw new ShopServiceException("There is no not enough product in any shop");
            return found[0].Id;
        }

        public void Buy(Buyer buyer, uint shopId, string productName, uint count)
        {
            Product product = _deliveryAgent.GetProduct(productName);
            GetShopById(shopId).Sell(buyer, product, count);
        }

        public IReadOnlyList<SellableProduct> GetShopProducts(uint shopId)
        {
            return GetShopById(shopId).Products;
        }

        private Shop GetShopById(uint shopId)
        {
            Shop? foundShop = _shops.Find(shop => shop.Id == shopId);
            if (foundShop is null)
                throw new ShopServiceException("There is no such shop");
            return foundShop;
        }
    }
}