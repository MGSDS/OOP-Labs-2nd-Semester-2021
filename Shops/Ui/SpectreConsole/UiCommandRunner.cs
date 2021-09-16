using System;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Shops.Ui.Interfaces;
using Spectre.Console;

namespace Shops.Ui.SpectreConsole
{
    public class UiCommandRunner : IUiCommandRunner
    {
        private ShopService _shopService;
        private Buyer _buyer;

        public UiCommandRunner(ShopService shopService, Buyer buyer)
        {
            _shopService = shopService;
            _buyer = buyer;
        }

        public void ShowBuyerProducts()
        {
            Table table = TableGenerator.CountableProductsTable(_buyer.Products);
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        public void ShowShops()
        {
            Table table = TableGenerator.ShopsTable(_shopService.Shops);
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        public void RegisterProduct()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]product name[/]?");
            _shopService.RegisterProduct(name);
        }

        public void RegisterShop()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]shop name[/]?");
            string address = AnsiConsole.Ask<string>("Enter [green]shop address[/]?");
            _shopService.RegisterShop(name, address);
        }

        public void ShowBuyerMoney()
        {
            AnsiConsole.Render(new Markup($"Buyer have [green]{_buyer.Money}[/]\n"));
            ReturnPrompt();
        }

        public void AddBuyerMoney()
        {
            uint money = AnsiConsole.Ask<uint>("How much you want to add?");
            _buyer.Money += money;
        }

        public void ShowRegisteredProducts()
        {
            Table table = TableGenerator.ProductsTable(_shopService.RegisteredProducts);
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        public void ShowShopProducts()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            Table table;
            try
            {
                table = TableGenerator.SellableProductsTable(_shopService.GetShopProducts(id));
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
                return;
            }

            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        public void AddProductToShop()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint count = AnsiConsole.Ask<uint>("Enter [green]products count[/]");
            uint price = AnsiConsole.Ask<uint>("Enter [green]product price[/]");
            try
            {
                _shopService.AddProduct(id, productName, count, price);
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
            }
        }

        public void FindCheapestShop()
        {
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint count = AnsiConsole.Ask<uint>("Enter [green]products count[/]");
            uint shopId;
            try
            {
                shopId = _shopService.FindCheapestPriceShopId(productName, count);
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
                return;
            }

            AnsiConsole.Markup(
                $"Cheapest {productName} found in shop with id [green]{shopId}[/] for [green]{_shopService.GetShopProducts(shopId).FirstOrDefault(product => product.CountableProduct.Product.Name == productName).Price}[/]\n");
            ReturnPrompt();
        }

        public void Buy()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint count = AnsiConsole.Ask<uint>("Enter [green]product count[/]");
            try
            {
                _shopService.Buy(_buyer, id, productName, count);
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
            }
        }

        public void ChangePrice()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint price = AnsiConsole.Ask<uint>("Enter new [green]product price[/]");
            try
            {
                _shopService.ChangePrice(id, productName, price);
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
            }
        }

        private void ReturnPrompt()
        {
            AnsiConsole.Render(new Text("Press Enter to return back"));
            Console.Read();
        }
    }
}