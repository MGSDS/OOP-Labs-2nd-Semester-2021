using System;
using System.Collections.Generic;
using Shops.Ui.DataTypes;
using Shops.Ui.Interfaces;
using Spectre.Console;

namespace Shops.Ui.SpectreConsole
{
    public class UiChoice : IUiChoice
    {
        private IUiCommandRunner _commandRunner;
        private List<Command> _commands;
        private bool _run;

        public UiChoice(IUiCommandRunner commandRunner)
        {
            _commandRunner = commandRunner;
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Register product", new Action(_commandRunner.RegisterProduct)),
                new Command("Register shop", new Action(_commandRunner.RegisterShop)),
                new Command("Add product to the shop", new Action(_commandRunner.AddProductToShop)),
                new Command("Change price", new Action(_commandRunner.ChangePrice)),
                new Command("Find cheapest shop", new Action(_commandRunner.FindCheapestShop)),
                new Command("Buy", new Action(_commandRunner.Buy)),
                new Command("Show shop products", new Action(_commandRunner.ShowShopProducts)),
                new Command("Show shops", new Action(_commandRunner.ShowShops)),
                new Command("Show registered products", new Action(_commandRunner.ShowRegisteredProducts)),
                new Command("Show my products", new Action(_commandRunner.ShowBuyerProducts)),
                new Command("Show my money", new Action(_commandRunner.ShowBuyerMoney)),
                new Command("Add money", new Action(_commandRunner.AddBuyerMoney)),
                new Command("exit", new Action(Exit)),
            });
        }

        public bool Show()
        {
            _run = true;
            AnsiConsole.Clear();
            AskAction().cmd();
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

        private void Exit()
        {
            _run = false;
        }
    }
}