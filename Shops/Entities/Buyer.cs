#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Buyer
    {
        private List<CountableProduct> _products;
        public Buyer()
            : this(0) { }
        public Buyer(uint money)
        {
            _products = new List<CountableProduct>();
            Money = money;
        }

        public uint Money { get; set; }
        public IReadOnlyList<CountableProduct> Products => _products;

        public void GiveProduct(CountableProduct product, uint cost)
        {
            if (cost > Money)
                throw new ShopServiceException("Buyer doesn't have enough money");
            if (_products.All(buyerProduct => !buyerProduct.Product.Equals(product.Product)))
                _products.Add((CountableProduct)product.Clone());
            else
                _products.Find(buyerProduct => buyerProduct.Product.Equals(product.Product)) !.Count += product.Count;
            Money -= cost;
        }
    }
}