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

        public Shop(uint id, string name, string address, List<SellableProduct> products)
        {
            _products = new List<SellableProduct>(products);
            Id = id;
            Name = name;
            Address = address;
        }

        public uint Id { get; }
        public string Name { get; }
        public string Address { get; }
        public IReadOnlyList<SellableProduct> Products => _products;

        public bool CheckAvailability(Product product, uint count)
        {
            SellableProduct? found = TryFindAvailableProduct(product, count);
            return found != null && found.CountableProduct.Count >= count;
        }

        public void Sell(Buyer buyer, Product product, uint count)
        {
            SellableProduct found = TryFindAvailableProduct(product, count) ??
                                     throw new ShopServiceException("Product is not available");

            buyer.GiveProduct(new CountableProduct(product, count), count * found.Price);

            found.CountableProduct.Count -= count;
        }

        public void ChangePrice(Product product, uint newPrice)
        {
            SellableProduct found = _products.FirstOrDefault(shopProduct => shopProduct.CountableProduct.Product.Equals(product)) ??
                                    throw new ShopServiceException("There is no such product in the shop");
            found.Price = newPrice;
        }

        public void GiveProduct(SellableProduct product)
        {
            SellableProduct? localProduct = _products.Find(shopProduct => shopProduct.CountableProduct.Product
                .Equals(product.CountableProduct.Product));
            if (localProduct is null)
            {
                _products.Add(product);
                return;
            }

            localProduct.CountableProduct.Count += product.CountableProduct.Count;
        }

        private SellableProduct? TryFindAvailableProduct(Product product, uint count)
        {
            SellableProduct? found = _products.Find(shopProduct => shopProduct.CountableProduct.Product.Equals(product));
            return found?.CountableProduct.Count < count ? null : found;
        }
    }
}