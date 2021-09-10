namespace Shops.Entities
{
    public class SellableProduct : Product
    {
        public SellableProduct(string name, uint price, uint count)
            : base(name, count)
        {
            Price = price;
        }

        public uint Price { get; set; }

        public override object Clone()
        {
            return new SellableProduct(Name, Price, Count);
        }
    }
}