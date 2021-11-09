using System;
using System.Collections.Generic;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Ui.Commands;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class ClientWindow
    {
        private ICommandHandler _handler;
        private List<Command> _commands;
        private bool _run;
        private Client _client;
        private Bank _bank;

        public ClientWindow(ICommandHandler handler, Guid clientId, Bank bank)
        {
            _bank = bank;
            _handler = handler;
            _client = _handler.GetClient(clientId);
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Show accounts", new Action(ShowAccounts)),
                new Command("Create debit account", new Action(CreateDebitAccount)),
                new Command("Create credit account", new Action(CreateCreditAccount)),
                new Command("Create deposit account", new Action(CreateDepositAccount)),
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

        private void CreateDepositAccount()
        {
            DateTime endDate = AnsiConsole.Ask<DateTime>("Enter [green]end date[/]");
            Guid id = _handler.CreateDepositAccount(_client, _bank, endDate);
            AnsiConsole.WriteLine($"[green]Account created. Id: {id}[/]");
            Prompts.ReturnPrompt();
        }

        private void CreateCreditAccount()
        {
            Guid id = _handler.CreateCreditAccount(_client, _bank);
            AnsiConsole.WriteLine($"[green]Account created. Id: {id}[/]");
            Prompts.ReturnPrompt();
        }

        private void CreateDebitAccount()
        {
            Guid id = _handler.CreateDebitAccount(_client, _bank);
            AnsiConsole.WriteLine($"[green]Account created. Id: {id}[/]");
            Prompts.ReturnPrompt();
        }

        private void ShowAccounts()
        {
            IReadOnlyList<AbstractAccount> accounts = _handler.GetClientAccounts(_client);
            Table table = TableGenerator.GenerateTable(accounts);
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }
    }
}