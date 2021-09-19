using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            IReadOnlyList<SellableProduct> shopProducts =  _shopService.Shops.FirstOrDefault(shop => shopId == shop.Id).Products;
            SellableProduct product = shopProducts.First(product => product.CountableProduct.Product.Name == productName);
            uint actualCount = product.CountableProduct.Count;
            Assert.AreEqual(count * 2, actualCount);
        }
        
        [Test]
        [TestCase(12U, 11U, 4U)]
        [TestCase(13U, 1000U, 4U)]
        public void ChangeProductPrice_ProductPriceChange(uint price, uint newPrice, uint count)
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, count, price);
            _shopService.ChangePrice(shopId, productName, newPrice);
            IReadOnlyList<SellableProduct> shopProducts =  _shopService.Shops.FirstOrDefault(shop => shopId == shop.Id).Products;
            SellableProduct product = shopProducts.First(product => product.CountableProduct.Product.Name == productName);
            Assert.AreEqual(newPrice, product.Price);
        }
        
        [Test]
        [TestCase(10U, 100U, 100U)]
        [TestCase(1U, 2U, 400U)]
        [TestCase( 231U, 599U, 1U)]
        public void FindCheapestPrice_ShopWithCheapestPriceReturned(uint lowestPrice, uint maxPrice, uint count)
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            uint cheapestShopId = _shopService.RegisterShop(shopName, "TestAddress");
            uint expensiveShopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(cheapestShopId, productName, count, lowestPrice);
            _shopService.AddProduct(expensiveShopId, productName, count, maxPrice);
            uint shopId = _shopService.FindCheapestPriceShopId(productName, count);
            IReadOnlyList<SellableProduct> shopProducts =  _shopService.Shops.FirstOrDefault(shop => shopId == shop.Id).Products;
            SellableProduct product = shopProducts.First(product => product.CountableProduct.Product.Name == productName);
            Assert.AreEqual(lowestPrice,product.Price);
        }
        
        [Test]
        [TestCase(1U, 2U, 400U)]
        [TestCase(2U, 200U, 10000U)]
        public void BuyProduct_BuyerHasProductMoneyDecreaseShopProductCountDecrease(uint price, uint count, uint money)
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            Shop shop = _shopService.Shops.FirstOrDefault(localShop => localShop.Id == shopId);
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, count, price);
            _shopService.Buy(buyer, shopId, productName, count / 2);
            Assert.AreEqual(money - count / 2 * price, buyer.Money);
            Assert.AreEqual(count / 2, shop.Products[0].CountableProduct.Count);
            Assert.AreEqual(count / 2, buyer.Products[0].Count);
            _shopService.Buy(buyer, shopId, productName, count / 2);
            Assert.AreEqual(money - count * price, buyer.Money);
            Assert.AreEqual(0, shop.Products[0].CountableProduct.Count);
            Assert.AreEqual(count, buyer.Products[0].Count);
        }
        
        [Test]
        [TestCase(1U, 2U, 0U)]
        [TestCase(6U, 400U, 1000U)]
        public void BuyProductBuyerHaveNotEnoughMoney_Exception(uint price, uint count, uint money)
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
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
        [TestCase(6U, 400U, 1000U, 100000U)]
        [TestCase(6U, 0U, 1U, 100000U)]
        public void BuyProductShopHasNotEnoughProducts_Exception(uint price, uint countInShop, uint countBuy, uint money)
        {
            const string productName = "TestProduct";
            const string shopName = "TestShop";
            var buyer = new Buyer(money);
            uint shopId = _shopService.RegisterShop(shopName, "TestAddress");
            _shopService.RegisterProduct(productName);
            _shopService.AddProduct(shopId, productName, countInShop, price);
            Assert.Catch<ShopServiceException>(() =>
            {
                _shopService.Buy(buyer, shopId, productName, countBuy);
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