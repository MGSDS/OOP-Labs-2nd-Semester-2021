using Shops.Interfaces;

namespace Shops.Entities
{
    public class CountableProduct : ICloneable<CountableProduct>
    {
        public CountableProduct(Product product, uint count)
        {
            Product = product;
            Count = count;
        }

        public Product Product { get; }
        public uint Count { get; set; }

        public CountableProduct Clone()
        {
            return new CountableProduct(Product, Count);
        }
    }
}