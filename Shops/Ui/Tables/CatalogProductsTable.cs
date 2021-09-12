using System.Collections.Generic;
using Shops.Entities;
using Spectre.Console;

namespace Shops
{
    public class CatalogProductsTable
    {
        public CatalogProductsTable()
        {
            Table = new Table();
            Table.AddColumn("Name");
        }

        public Table Table { get; }
        public void AddProduct(CatalogProduct product)
        {
            Table.AddRow(product.Name);
        }

        public void AddProducts(IReadOnlyList<CatalogProduct> products)
        {
            foreach (CatalogProduct product in products)
            {
                AddProduct(product);
            }
        }
    }
}