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

        public CatalogProduct RegisterProduct(string productName)
        {
            _deliveryAgent.RegisterProduct(productName);
            return Catalog.FirstOrDefault(product => product.Name == productName) !;
        }

        public uint RegisterShop(string shopName, string address)
        {
            _shops.Add(new Shop(_maxId++, shopName, address));
            return _maxId - 1;
        }

        public void AddProduct(uint shopId, string productName, uint count, uint price)
        {
            CheckProductRegistration(productName);
            _deliveryAgent.DeliverProductToShop(GetShopById(shopId), new SellableProduct(productName, price, count));
        }

        public void ChangePrice(uint shopId, string productName, uint newPrice)
        {
            Shop shop = GetShopById(shopId);
            CheckProductRegistration(productName);
            shop.Products.FirstOrDefault(product => product.Name == productName) !.Price = newPrice;
        }

        public uint FindCheapestPriceShopId(string productName, uint count)
        {
            CheckProductRegistration(productName);
            List<Shop> found = Shops.Where(shop => shop.CheckAvailability(productName, count))
                .OrderBy(shop => shop.Products.FirstOrDefault(product => product.Name == productName) !.Price).ToList();
            if (!found.Any())
                throw new ShopServiceException("There is no not enough product in any shop");
            return found[0].Id;
        }

        public void Buy(Buyer buyer, uint shopId, string productName, uint count)
        {
            CheckProductRegistration(productName);
            GetShopById(shopId).Sell(buyer, productName, count);
        }

        private Shop GetShopById(uint shopId)
        {
            CheckShopRegistration(shopId);
            return _shops.FirstOrDefault(listedShop => listedShop.Id == shopId) !;
        }

        private void CheckProductRegistration(string productName)
        {
            if (Catalog.All(product => product.Name != productName))
                throw new ShopServiceException("There is no such product registered");
        }

        private void CheckShopRegistration(uint shopId)
        {
            if (Shops.All(shop => shop.Id != shopId))
                throw new ShopServiceException("There is no such shop registered");
        }
    }
}