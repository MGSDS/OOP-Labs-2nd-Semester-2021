using System.Collections.Generic;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Ui.Interfaces;

namespace Shops.Ui.SpectreConsole
{
    public class CommandHandler : ICommandHandler
    {
        private ShopService _shopService;
        private Buyer _buyer;

        public CommandHandler(ShopService shopService, Buyer buyer)
        {
            _shopService = shopService;
            _buyer = buyer;
        }

        public IReadOnlyList<CountableProduct> GetBuyerProducts()
        {
            return _buyer.Products;
        }

        public IReadOnlyList<Shop> GetShops()
        {
            return _shopService.Shops;
        }

        public void RegisterProduct(string name)
        {
            _shopService.RegisterProduct(name);
        }

        public void RegisterShop(string name, string address)
        {
            _shopService.RegisterShop(name, address);
        }

        public uint GetBuyerMoney()
        {
            return _buyer.Money;
        }

        public void AddBuyerMoney(uint money)
        {
            _buyer.Money += money;
        }

        public IReadOnlyList<Product> GetRegisteredProducts()
        {
            return _shopService.RegisteredProducts;
        }

        public IReadOnlyList<SellableProduct> GetShopProducts(uint id)
        {
            return _shopService.GetShopProducts(id);
        }

        public void AddProductToShop(uint shopId, string productName, uint count, uint price)
        {
            _shopService.AddProduct(shopId, productName, count, price);
        }

        public uint FindCheapestShop(string productName, uint count)
        {
            return _shopService.FindCheapestPriceShopId(productName, count);
        }

        public void Buy(uint id, string productName, uint count)
        {
            _shopService.Buy(_buyer, id, productName, count);
        }

        public uint GetProductPrice(uint shopId, string productName)
        {
            return _shopService.GetShopProducts(shopId).FirstOrDefault(product => product.CountableProduct.Product.Name == productName).Price;
        }

        public void ChangePrice(uint id, string productName, uint price)
        {
            _shopService.ChangePrice(id, productName, price);
        }
    }
}