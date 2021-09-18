using System;
using System.Collections.Generic;
using Shops.Tools;
using Shops.Ui.DataTypes;
using Shops.Ui.Interfaces;
using Spectre.Console;

namespace Shops.Ui.SpectreConsole
{
    public class UiCommandRunner
    {
        private ICommandHandler _commandHandler;
        private List<Command> _commands;
        private bool _run;

        public UiCommandRunner(ICommandHandler commandhandler)
        {
            _commandHandler = commandhandler;
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Register product", new Action(RegisterProduct)),
                new Command("Register shop", new Action(RegisterShop)),
                new Command("Add product to the shop", new Action(AddProductToShop)),
                new Command("Change price", new Action(ChangePrice)),
                new Command("Find cheapest shop", new Action(FindCheapestShop)),
                new Command("Buy", new Action(Buy)),
                new Command("Show shop products", new Action(ShowShopProducts)),
                new Command("Show shops", new Action(ShowShops)),
                new Command("Show registered products", new Action(ShowRegisteredProducts)),
                new Command("Show my products", new Action(ShowBuyerProducts)),
                new Command("Show my money", new Action(ShowBuyerMoney)),
                new Command("Add money", new Action(AddBuyerMoney)),
                new Command("exit", new Action(Exit)),
            });
        }

        public bool ChooseAction()
        {
            _run = true;
            AnsiConsole.Clear();
            AskAction().Action();
            return _run;
        }

        private Command AskAction()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<Command>()
                    .Title("[yellow]Choose action[/]")
                    .PageSize(10)
                    .AddChoices(_commands));
        }

        private void ShowBuyerProducts()
        {
            Table table = TableGenerator.CountableProductsTable(_commandHandler.GetBuyerProducts());
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        private void ShowShops()
        {
            Table table = TableGenerator.ShopsTable(_commandHandler.GetShops());
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        private void RegisterProduct()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]product name[/]?");
            _commandHandler.RegisterProduct(name);
        }

        private void RegisterShop()
        {
            string name = AnsiConsole.Ask<string>("Enter [green]shop name[/]?");
            string address = AnsiConsole.Ask<string>("Enter [green]shop address[/]?");
            _commandHandler.RegisterShop(name, address);
        }

        private void ShowBuyerMoney()
        {
            AnsiConsole.Render(new Markup($"Buyer have [green]{_commandHandler.GetBuyerMoney()}[/]\n"));
            ReturnPrompt();
        }

        private void AddBuyerMoney()
        {
            uint money = AnsiConsole.Ask<uint>("How much you want to add?");
            _commandHandler.AddBuyerMoney(money);
        }

        private void ShowRegisteredProducts()
        {
            Table table = TableGenerator.ProductsTable(_commandHandler.GetRegisteredProducts());
            AnsiConsole.Render(table);
            ReturnPrompt();
        }

        private void ShowShopProducts()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            Table table;
            try
            {
                table = TableGenerator.SellableProductsTable(_commandHandler.GetShopProducts(id));
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

        private void AddProductToShop()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint count = AnsiConsole.Ask<uint>("Enter [green]products count[/]");
            uint price = AnsiConsole.Ask<uint>("Enter [green]product price[/]");
            try
            {
                _commandHandler.AddProductToShop(id, productName, count, price);
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
                shopId = _commandHandler.FindCheapestShop(productName, count);
            }
            catch (ShopServiceException e)
            {
                AnsiConsole.Render(new Markup($"[red]{e.Message}[/]\n"));
                ReturnPrompt();
                return;
            }

            AnsiConsole.Markup(
                $"Cheapest {productName} found in shop with id [green]{shopId}[/] for [green]{_commandHandler.GetProductPrice(shopId, productName)}[/]\n");
            ReturnPrompt();
        }

        private void Buy()
        {
            uint id = AnsiConsole.Ask<uint>("Enter [green]shop id[/]");
            string productName = AnsiConsole.Ask<string>("Enter [green]product name[/]");
            uint count = AnsiConsole.Ask<uint>("Enter [green]product count[/]");
            try
            {
                _commandHandler.Buy(id, productName, count);
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
                _commandHandler.ChangePrice(id, productName, price);
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

        private void Exit()
        {
            _run = false;
        }
    }
}