#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private List<SellableProduct> _products;

        public Shop(uint id, string name, string address)
        {
            _products = new List<SellableProduct>();
            Id = id;
            Name = name;
            Address = address;
        }

        public uint Id { get; }
        public string Name { get; }
        public string Address { get; }
        public IReadOnlyList<SellableProduct> Products => _products;

        public bool CheckAvailability(Product product)
        {
            SellableProduct? found = _products.FirstOrDefault(shopProduct => shopProduct.Name == product.Name);
            return found != null && found.Count >= product.Count;
        }

        public void Sell(Buyer buyer, Product product)
        {
            SellableProduct found = _products.FirstOrDefault(shopProduct => shopProduct.Name == product.Name) ??
                                     throw new ShopServiceException("There is no products in this shop");
            if (found.Count < product.Count)
                throw new ShopServiceException("There is no enough products in this shop");

            buyer.GiveProduct(new Product(product.Name, product.Count), product.Count * found.Price);

            found.Count -= product.Count;
            if (found.Count == 0)
                _products.Remove(found);
        }

        public void ChangePrice(string productName, uint newPrice)
        {
            SellableProduct found = _products.FirstOrDefault(shopProduct => shopProduct.Name == productName) ??
                                    throw new ShopServiceException("There is no such product");
            found.Price = newPrice;
        }

        public void GiveProduct(SellableProduct product)
        {
            if (_products.All(shopProduct => shopProduct.Name != product.Name))
            {
                _products.Add((SellableProduct)product.Clone());
                return;
            }

            _products.FirstOrDefault(shopProduct => shopProduct.Name == product.Name) !.Count += product.Count;
        }
    }
}