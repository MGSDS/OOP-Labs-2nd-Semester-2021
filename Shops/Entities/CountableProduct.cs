using System;

namespace Shops.Entities
{
    public class CountableProduct : ICloneable
    {
        public CountableProduct(Product product, uint count)
        {
            Product = product;
            Count = count;
        }

        public Product Product { get; }
        public uint Count { get; set; }
        public object Clone()
        {
            return new CountableProduct(Product, Count);
        }
    }
}