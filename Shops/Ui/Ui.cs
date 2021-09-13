using System;
using System.Linq;
using Shops.Entities;
using Shops.Services;
using Shops.Tools;
using Spectre.Console;

namespace Shops
{
    public class Ui
    {
        private CommandHandler _commandHandler;

        public Ui(ShopService shopService, Buyer buyer)
        {
            _commandHandler = new CommandHandler(shopService, buyer);
        }

        public void Run()
        {
            while (true)
            {
                AnsiConsole.Clear();
                string command = AskAction();
                if (command == "exit")
                    return;
                _commandHandler.Run(command);
            }
        }

        private string AskAction()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Choose action[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "Register product",
                        "Register shop",
                        "Add product to the shop",
                        "Change price",
                        "Find cheapest shop",
                        "Buy",
                        "Show shop products",
                        "Show shops",
                        "Show registered products",
                        "Show my products",
                        "Show my money",
                        "Add money",
                        "exit",
                    }));
        }
    }
}