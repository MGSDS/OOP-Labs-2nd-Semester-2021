using System;
using System.Collections.Generic;

namespace Shops.Entities
{
    public class CatalogProduct
    {
        public CatalogProduct(Product product)
        {
            Product = product;
            Shops = new List<Shop>();
        }

        public Product Product { get; }
        public List<Shop> Shops { get; set; }
    }
}