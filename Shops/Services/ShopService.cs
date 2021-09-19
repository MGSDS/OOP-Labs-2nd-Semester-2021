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
        private List<CatalogProduct> _catalog;
        private uint _maxId;

        public ShopService()
        {
            _maxId = 0;
            _shops = new List<Shop>();
            _deliveryAgent = new DeliveryAgent();
            _catalog = new List<CatalogProduct>();
        }

        public ShopService(DeliveryAgent deliveryAgent)
        {
            _maxId = 0;
            _shops = new List<Shop>();
            _deliveryAgent = deliveryAgent.Clone();
            _catalog = new List<CatalogProduct>();
            foreach (Product product in _deliveryAgent.Catalog)
            {
                _catalog.Add(new CatalogProduct(product));
            }
        }

        public IReadOnlyList<Shop> Shops => _shops;
        public IReadOnlyList<Product> RegisteredProducts => _deliveryAgent.Catalog;
        public IReadOnlyList<CatalogProduct> Catalog => _catalog;

        public Product RegisterProduct(string productName)
        {
            Product product = _deliveryAgent.RegisterProduct(productName);
            _catalog.Add(new CatalogProduct(product));
            return product;
        }

        public uint RegisterShop(string shopName, string address)
        {
            _shops.Add(new Shop(_maxId++, shopName, address));
            return _maxId - 1;
        }

        public void AddProduct(uint shopId, string productName, uint count, uint price)
        {
            Shop shop = GetShopById(shopId);
            Product product = _deliveryAgent.GetProduct(productName);
            _deliveryAgent.DeliverProductToShop(shop, new SellableProduct(new CountableProduct(product, count), price));
            _catalog.Find(catalogProduct => catalogProduct.Product.Name == productName) !.Shops.Add(shop);
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
            IEnumerable<Shop>? shops = _catalog.Find(catalogProduct => catalogProduct.Product.Equals(product))?.Shops.Where(shop => shop.CheckAvailability(product, count));
            if (shops is null)
                throw new ShopServiceException("Product is not registered");
            Shop? found = shops.OrderBy(shop => shop.Products.First(sellableProduct => sellableProduct.CountableProduct.Product.Equals(product)).Price).FirstOrDefault();
            if (found is null)
                throw new ShopServiceException("There is no not enough product in any shop");
            return found.Id;
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