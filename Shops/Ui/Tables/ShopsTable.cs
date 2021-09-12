using System.Collections.Generic;
using Shops.Entities;
using Spectre.Console;

namespace Shops
{
    public class ShopsTable
    {
        public ShopsTable()
        {
            Table = new Table();
            Table.AddColumn("[red]Id[/]");
            Table.AddColumn("Name");
            Table.AddColumn("Address");
        }

        public Table Table { get; }
        public void AddProduct(Shop shop)
        {
            Table.AddRow($"[red]{shop.Id}[/]", shop.Name, shop.Address);
        }

        public void AddProducts(IReadOnlyList<Shop> shops)
        {
            foreach (Shop shop in shops)
            {
                AddProduct(shop);
            }
        }
    }
}