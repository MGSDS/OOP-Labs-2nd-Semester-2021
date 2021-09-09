namespace Shops.Entities
{
    public class SellableProduct : Product
    {
        public SellableProduct(string name, uint price, uint count)
            : base(name, count)
        {
            Cost = price;
        }

        public uint Cost { get; set; }

        public override object Clone()
        {
            return new SellableProduct(Name, Cost, Count);
        }
    }
}