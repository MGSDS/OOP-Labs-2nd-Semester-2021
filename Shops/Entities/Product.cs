namespace Shops.Entities
{
    public class Product : CatalogProduct
    {
        public Product(string name, uint count)
            : base(name)
        {
            Count = count;
        }

        public uint Count { get; set; }

        public override object Clone()
        {
            return new Product(Name, Count);
        }
    }
}