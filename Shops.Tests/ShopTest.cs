using System;
using NUnit.Framework;
using Shops.Entities;

namespace Shops.Tests
{
    public class ShopTest
    {
        private Shop _shop;
        
        [SetUp]
        public void Setup()
        {
            _shop = new Shop(0, "TestShop");
        }

        [Test]
        public void CheckAvailableProductAvailability_True()
        {
            var testProduct = new SellableProduct("TestProduct", 10, 100);
            _shop.GiveProduct(testProduct);
            Assert.True(_shop.CheckAvailability(testProduct));
        }
        
        [Test]
        public void CheckUnavailableProductAvailability_False()
        {
            var testProduct = new Product("TestProduct", 100);
            Assert.False(_shop.CheckAvailability(testProduct));   
        }

        [Test]
        public void CheckUnderCountProductAvailability_False()
        {
            var testProduct1 = new SellableProduct("TestProduct", 10, 100);
            var testProduct2 = new Product("TestProduct", 1000);
            _shop.GiveProduct(testProduct1);
            Assert.False(_shop.CheckAvailability(testProduct2));   
        }
        
        [Test]
        public void ChangePriceProductExists_PriceChanges()
        {
            _shop.GiveProduct(new SellableProduct("TestProduct", 1, 1));
            _shop.ChangePrice("TestProduct", 1000);
            Assert.AreEqual(1000, _shop.Products[0].Cost);   
        }
        
        [Test]
        public void ChangePriceProductNotExists_Exception()
        {
            Assert.Catch<Exception>(() =>
            {
                _shop.ChangePrice("TestProduct", 1000);
            });
        }
    }
}