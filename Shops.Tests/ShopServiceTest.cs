using System;
using System.Linq;
using NUnit.Framework;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;

namespace Shops.Tests
{
    public class ShopServiceTest
    {
        private ShopService _shopService;
        
        [SetUp]
        public void Setup()
        {
            _shopService = new ShopService();
        }

        [Test]
        public void GiveShopProduct_ProductIsInShop()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 10;
            const uint count = 100;
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, count, price);
            _shopService.AddProduct(shopId, productName, count, price); 
            Assert.AreEqual(count*2, _shopService.Shops.FirstOrDefault(shop => shopId == shop.Id)
                .Products.FirstOrDefault(product => product.Name == productName).Count);
        }
        
        [Test]
        public void ChangeProductPrice_ProductPriceChange()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 10;
            const uint newPrice = 100;
            const uint count = 100;
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, count, price);
            _shopService.ChangePrice(shopId, productName, newPrice);
            Assert.AreEqual(newPrice, _shopService.Shops.FirstOrDefault(shop => shopId == shop.Id)
                .Products.FirstOrDefault(listedProduct => listedProduct.Name == productName).Price);
        }
        
        [Test]
        public void FindCheapestPrice_ShopWithCheapestPriceReturned()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint lowestPrice = 10;
            const uint maxPrice = 100;
            const uint count = 100;
            uint cheapestShopId = _shopService.RegisterShop(shopName, "TestAddress");
            uint expensiveShopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(cheapestShopId, productName, count, lowestPrice);
            _shopService.AddProduct(expensiveShopId, productName, count, maxPrice);
            uint found = _shopService.FindCheapestPriceShopId(productName, count);
            Assert.AreEqual(lowestPrice,
                _shopService.Shops.FirstOrDefault(shop => shop.Id == found).Products
                    .FirstOrDefault(product => product.Name == productName).Price);
        }
        
        [Test]
        public void BuyProduct_BuyerHasProductMoneyDecreaseShopProductCountDecrease()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 1;
            const uint count = 100;
            const uint money = 1000000;
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            Shop shop = _shopService.Shops.FirstOrDefault(localShop => localShop.Id == shopId);
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, count, price);
            _shopService.Buy(buyer, shopId, productName, count / 2);
            Assert.AreEqual(money - count / 2 * price, buyer.Money);
            Assert.AreEqual(count / 2, shop.Products[0].Count);
            Assert.AreEqual(count / 2, buyer.Products[0].Count);
            _shopService.Buy(buyer, shopId, productName, count / 2);
            Assert.AreEqual(money - count * price, buyer.Money);
            Assert.AreEqual(false, shop.Products.Any());
            Assert.AreEqual(count, buyer.Products[0].Count);
        }
        
        [Test]
        public void BuyProductBuyerHaveNotEnoughMoney_Exception()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 1;
            const uint count = 100;
            const uint money = 0;
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, price, count);
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shopId, productName, count);
            });

        }
        
        [Test]
        public void BuyProductShopHasNotEnoughProducts_Exception()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 1;
            const uint count = 100;
            const uint money = 10000;
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, price, count);
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shopId, productName, count + 1);
            });
        }
        
        [Test]
        public void BuyProductShopHasNoProduct_Exception()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint count = 100;
            const uint money = 10000;
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shopId, productName, count);
            });

        }
    }
}