using System;

namespace Shops.Entities
{
    public class CatalogProduct : ICloneable
    {
        public CatalogProduct(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public virtual object Clone()
        {
            return new CatalogProduct(Name);
        }
    }
}