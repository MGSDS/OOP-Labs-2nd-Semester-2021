using Shops.Interfaces;

namespace Shops.Entities
{
    public class SellableProduct : ICloneable<SellableProduct>
    {
        public SellableProduct(CountableProduct product, uint price)
        {
            Price = price;
            CountableProduct = product;
        }

        public uint Price { get; set; }
        public CountableProduct CountableProduct { get; }

        public SellableProduct Clone()
        {
            return new SellableProduct(CountableProduct.Clone(), Price);
        }
    }
}