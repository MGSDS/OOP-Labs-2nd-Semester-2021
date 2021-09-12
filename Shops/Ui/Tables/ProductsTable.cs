using System.Collections.Generic;
using Shops.Entities;
using Spectre.Console;

namespace Shops
{
    public class ProductsTable
    {
        public ProductsTable()
        {
            Table = new Table();
            Table.AddColumn("Name");
            Table.AddColumn("Count");
        }

        public Table Table { get; }
        public void AddProduct(Product product)
        {
            Table.AddRow(product.Name, product.Count.ToString());
        }

        public void AddProducts(IReadOnlyList<Product> products)
        {
            foreach (Product product in products)
            {
                AddProduct(product);
            }
        }
    }
}