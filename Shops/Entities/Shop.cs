#nullable enable
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

        public bool CheckAvailability(string productName, uint count)
        {
            SellableProduct? found = _products.FirstOrDefault(product => product.Name == productName);
            return found != null && found.Count >= count;
        }

        public void Sell(Buyer buyer, string productName, uint count)
        {
            SellableProduct found = _products.FirstOrDefault(product => productName == product.Name) ??
                                     throw new ShopServiceException("There is no products in this shop");
            if (found.Count < count)
                throw new ShopServiceException("There is no enough products in this shop");

            buyer.GiveProduct(new Product(productName, count), count * found.Price);

            found.Count -= count;
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