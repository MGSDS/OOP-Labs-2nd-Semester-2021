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
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shop, new SellableProduct(productName, price, count));
            _shopService.AddProduct(shop, new SellableProduct(productName, price, count));
            Assert.AreEqual(count*2, _shopService.Shops.FirstOrDefault(listedShop => listedShop.Id == shop.Id)
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
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            var product = new SellableProduct(productName, price, count);
            _shopService.AddProduct(shop, product);
            _shopService.ChangePrice(shop, product, newPrice);
            Assert.AreEqual(newPrice, _shopService.Shops.FirstOrDefault(listedShop => listedShop.Id == shop.Id)
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
            Shop cheapestShop = _shopService.RegisterShop(shopName, "TestAddress");
            Shop expensiveShop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            var cheapestProduct = new SellableProduct(productName, lowestPrice, count);
            var expensiveProduct = new SellableProduct(productName, maxPrice, count);
            _shopService.AddProduct(expensiveShop, expensiveProduct);
            _shopService.AddProduct(cheapestShop, cheapestProduct);
            Shop found = _shopService.FindCheapestPrice(new Product(productName, count));
            Assert.AreEqual(lowestPrice, found.Products.FirstOrDefault(product => product.Name == productName).Price);
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
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            Product product = _shopService.AddProduct(shop, new SellableProduct(productName, price, count));
            _shopService.Buy(buyer, shop, new Product(productName, count / 2));
            Assert.AreEqual(money - count / 2 * price, buyer.Money);
            Assert.AreEqual(count / 2, shop.Products[0].Count);
            Assert.AreEqual(count / 2, buyer.Products[0].Count);
            _shopService.Buy(buyer, shop, new Product(productName, count / 2));
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
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            Product product = _shopService.AddProduct(shop, new SellableProduct(productName, price, count));
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shop, new Product(productName, count));
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
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            Product product = _shopService.AddProduct(shop, new SellableProduct(productName, price, count));
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shop, new Product(productName, count + 1));
            });
        }
        
        [Test]
        public void BuyProductShopHasNoProduct_Exception()
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            const uint price = 1;
            const uint count = 100;
            const uint money = 10000;
            var buyer = new Buyer(money);
            Shop shop = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shop, new Product(productName, 100));
            });

        }
    }
}