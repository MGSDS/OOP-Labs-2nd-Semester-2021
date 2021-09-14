using System;

namespace Shops.Entities
{
    public class SellableProduct : ICloneable
    {
        public SellableProduct(CountableProduct product, uint price)
        {
            Price = price;
            CountableProduct = product;
        }

        public uint Price { get; set; }
        public CountableProduct CountableProduct { get; }

        public object Clone()
        {
            return new SellableProduct(CountableProduct, Price);
        }
    }
}