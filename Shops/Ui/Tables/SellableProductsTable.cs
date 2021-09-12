using System.Collections.Generic;
using Shops.Entities;
using Spectre.Console;

namespace Shops
{
    public class SellableProductsTable
    {
        public SellableProductsTable()
        {
            Table = new Table();
            Table.AddColumn("Name");
            Table.AddColumn("Count");
            Table.AddColumn("Price");
        }

        public Table Table { get; }
        public void AddProduct(SellableProduct product)
        {
            Table.AddRow(product.Name, product.Count.ToString(), product.Price.ToString());
        }

        public void AddProducts(IReadOnlyList<SellableProduct> products)
        {
            foreach (SellableProduct product in products)
            {
                AddProduct(product);
            }
        }
    }
}