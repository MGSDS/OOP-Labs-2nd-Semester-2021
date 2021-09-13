using System;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Spectre.Console;

namespace Shops
{
    public class CommandHandler
    {
        private ShopService _shopService;
        private Buyer _buyer;

        public CommandHandler(ShopService shopService, Buyer buyer)
        {
            _shopService = shopService;
            _buyer = buyer;
        }

        public void Run(string command)
        {
            switch (command)
            {
                case "Register product":
                    RegisterProduct();
                    break;
                case "Register shop":
                    RegisterShop();
                    break;
                case "Add product to the shop":
                    AddProductToShop();
                    break;
                case "Change price":
                    ChangePrice();
                    break;
                case "Find cheapest shop":
                    FindCheapestShop();
                    break;
                case "Buy":
                    Buy();
                    break;
                case "Show shop products":
                    ShowShopProducts();
                    break;
                case "Show shops":
                    ShowShops();
                    break;
                case "Show registered products":
                    ShowRegisteredProducts();
                    break;
                case "Show my products":
                    ShowBuyerProduct();
                    break;
                case "Show my money":
                    ShowBuyerMoney();
                    break;
                case "Add money":
                    AddBuyerMoney();
                    break;
            }
        }

        private void ShowBuyerProduct()
        {
            var table = new ProductsTable();
            table.AddProducts(_buyer.Products);
            AnsiConsole.Render(table.Table);
            ReturnPrompt();
        }

        private void ShowShops()
        {
            var table = new ShopsTable();
            table.AddProducts(_shopService.Shops);
            AnsiConsole.Render(table.Table);
            ReturnPrompt();
        }

        private void RegisterProduct()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]product name[/]?");
            _shopService.RegisterProduct(name);
        }

        private void RegisterShop()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]shop name[/]?");
            string address = AnsiConsole.Ask<string>("Enter [green]shop address[/]?");
            _shopService.RegisterShop(name, address);
        }

        private void ShowBuyerMoney()
        {
            AnsiConsole.Render(new Markup($"Buyer have [green]{_buyer.Money}[/]\n"));
            ReturnPrompt();
        }

        private void AddBuyerMoney()
        {
            uint money = AnsiConsole.Ask<uint>("How much you want to add?");
            _buyer.Money += money;
        }

        private void ShowRegisteredProducts()
        {
            var table = new CatalogProductsTable();
            table.AddProducts(_shopService.Catalog);
            AnsiConsole.Render(table.Table);
            ReturnPrompt();
        }

        private void ShowShopProducts()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            var table = new SellableProductsTable();
            try
            {
                table.AddProducts(_shopService.GetShopProducts(id));
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
                return;
            }

            AnsiConsole.Render(table.Table);
            ReturnPrompt();
        }

        private void AddProductToShop()
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

        private void FindCheapestShop()
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
                $"Cheapest {productName} found in shop with id [green]{shopId}[/] for [green]{_shopService.GetShopProducts(shopId).FirstOrDefault(product => product.Name == productName).Price}[/]\n");
            ReturnPrompt();
        }

        private void Buy()
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

        private void ChangePrice()
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