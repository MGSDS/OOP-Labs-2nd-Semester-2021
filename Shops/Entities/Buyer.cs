#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Buyer
    {
        private List<Product> _products;
        public Buyer()
            : this(0) { }
        public Buyer(uint money)
        {
            _products = new List<Product>();
            Money = money;
        }

        public uint Money { get; set; }
        public IReadOnlyList<Product> Products => _products;

        public void GiveProduct(Product product, uint cost)
        {
            if (cost > Money)
                throw new ShopServiceException("Buyer doesn't have enough money");
            Money -= cost;
            if (_products.All(buyerProduct => buyerProduct.Name != product.Name))
                _products.Add((Product)product.Clone());
            else
                _products.First(buyerProduct => buyerProduct.Name == product.Name).Count += product.Count;
        }
    }
}