using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Ui.Commands;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class AccountWindow
    {
        private ICommandHandler _handler;
        private List<Command> _commands;
        private bool _run;
        private Bank _bank;
        private AbstractAccount _account;

        public AccountWindow(ICommandHandler handler, Bank bank, Guid accountId)
        {
            _handler = handler;
            _bank = bank;
            _account = _handler.GetAccount(accountId);
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Withdraw", new Action(Withdraw)),
                new Command("Accrue", new Action(Accrue)),
                new Command("Transfer", new Action(Transfer)),
                new Command("Get balance", new Action(GetBalance)),
                new Command("Exit", new Action(() =>
                {
                    _run = false;
                })),
            });
        }

        public void Run()
        {
            do
            {
                ChooseAction();
            }
            while (_run);
        }

        private bool ChooseAction()
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

        private void Accrue()
        {
            decimal amount = AskAmount();
            _handler.Accrue(_bank, _account, amount);
        }

        private void Transfer()
        {
            Guid destinationAccountId = AnsiConsole.Ask<Guid>("Enter [green]destination account id[/]");
            decimal amount = AskAmount();
            _handler.Transfer(_account, _bank, destinationAccountId, amount);
        }

        private void Withdraw()
        {
            decimal amount = AskAmount();
            _handler.Withdraw(_bank, _account, amount);
        }

        private void GetBalance()
        {
            AnsiConsole.Write($"Balance {_account.Balance}");
            Prompts.ReturnPrompt();
        }

        private decimal AskAmount()
        {
            decimal amount = -1;
            while (amount < 0)
            {
                amount = AnsiConsole.Ask<decimal>("Enter [green]amount[/]");
                if (amount < 0)
                {
                    Console.Clear();
                    AnsiConsole.Write("[red]Amount can not be less 0[/]");
                }
            }

            return amount;
        }
    }
}