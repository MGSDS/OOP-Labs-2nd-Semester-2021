namespace Shops.Entities
{
    public class CatalogProduct : IProduct
    {
        public CatalogProduct(string name)
        {
            Name = name;
        }

        public override object Clone()
        {
            return new CatalogProduct(Name);
        }
    }
}