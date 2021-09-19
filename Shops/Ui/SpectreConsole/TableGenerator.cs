using System.Collections.Generic;
using Shops.Entities;
using Spectre.Console;

namespace Shops
{
    public static class TableGenerator
    {
        public static Table ProductsTable(IReadOnlyList<Product> products)
        {
            var table = new Table();
            table.AddColumn("Name");
            foreach (Product product in products)
            {
                table.AddRow(product.Name);
            }

            return table;
        }

        public static Table CountableProductsTable(IReadOnlyList<CountableProduct> products)
        {
            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Count");
            foreach (CountableProduct product in products)
            {
                table.AddRow(product.Product.Name, product.Count.ToString());
            }

            return table;
        }

        public static Table SellableProductsTable(IReadOnlyList<SellableProduct> products)
        {
            var table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Price");
            table.AddColumn("Count");
            foreach (SellableProduct product in products)
            {
                table.AddRow(product.CountableProduct.Product.Name, product.Price.ToString(), product.CountableProduct.Count.ToString());
            }

            return table;
        }

        public static Table ShopsTable(IReadOnlyList<Shop> shops)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Name");
            table.AddColumn("Address");
            foreach (Shop shop in shops)
            {
                table.AddRow(shop.Id.ToString(), shop.Name, shop.Address);
            }

            return table;
        }
    }
}