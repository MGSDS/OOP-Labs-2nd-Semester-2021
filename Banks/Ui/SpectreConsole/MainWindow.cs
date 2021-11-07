using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Ui.Commands;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class MainWindow
    {
        private ICommandHandler _handler;
        private List<Command> _commands;
        private bool _run;

        public MainWindow(ICommandHandler handler)
        {
            _handler = handler;
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Register Bank", new Action(RegisterBank)),
                new Command("Go to bank", new Action(GoToBank)),
                new Command("Go to transactions service", new Action(GoToTransactionsService)),
                new Command("ShowBanks", new Action(ShowBanks)),
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
                try
                {
                    ChooseAction();
                }
                catch (Exception e)
                {
                    AnsiConsole.WriteException(e);
                    Prompts.ReturnPrompt();
                }
            }
            while (_run);
        }

        private void GoToBank()
        {
            Guid id = Prompts.AskGuid("bank id");
            var bankWindow = new BankWindow(_handler, id);
            bankWindow.Run();
        }

        private void GoToTransactionsService()
        {
            var transactionsWindow = new TransactionsWindow(_handler);
            transactionsWindow.Run();
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

        private void ShowBanks()
        {
            Table table = TableGenerator.GenerateTable(_handler.GetBanks());
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }

        private void RegisterBank()
        {
            string name = AnsiConsole.Ask<string>($"Enter [green]bank name[/]?");
            decimal unverifiedLimit = AnsiConsole.Ask<decimal>($"Enter [green]unverified limit[/]?");
            decimal creditLimit = AnsiConsole.Ask<decimal>($"Enter [green]credit limit[/]?");
            decimal creditCommission = AnsiConsole.Ask<decimal>($"Enter [green]credit commission[/]?");
            decimal debitInterest = AnsiConsole.Ask<decimal>($"Enter [green]debit interest in %[/]?");
            ushort differentPercentageDebitCount = AnsiConsole.Ask<ushort>($"Enter [green]count of deposit account percentage steps[/]?");
            var depositInterests = new List<Interest>();
            for (int i = 0; i < differentPercentageDebitCount; i++)
            {
                decimal minimal = AnsiConsole.Ask<decimal>($"Enter [green]{i} minimal balance[/]?");
                decimal percentage = AnsiConsole.Ask<decimal>($"Enter [green]{i} interest in %[/]?");
                depositInterests.Add(new Interest(minimal, percentage));
            }

            Guid id = _handler.RegisterBank(name, unverifiedLimit, creditLimit, creditCommission, debitInterest, depositInterests);
            AnsiConsole.WriteLine($"Bank [green]{id}[/] registered");
        }
    }
}