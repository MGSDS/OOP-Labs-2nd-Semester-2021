using System;

namespace Shops.Entities
{
    public abstract class IProduct : ICloneable
    {
        public string Name { get; set; }
        public abstract object Clone();
    }
}