using System;
using System.Collections.Generic;
using Banks.Builders;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Ui.Commands;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class BankWindow
    {
        private ICommandHandler _handler;
        private List<Command> _commands;
        private bool _run;
        private Bank _bank;

        public BankWindow(ICommandHandler handler, Guid bankId)
        {
            _handler = handler;
            _bank = _handler.GetBank(bankId);
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Add client", new Action(AddClient)),
                new Command("Show clients", new Action(ShowClients)),
                new Command("Show accounts", new Action(ShowAccounts)),
                new Command("Go to client", new Action(GoToClient)),
                new Command("Go to account", new Action(GoToAccount)),
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

        private void GoToAccount()
        {
            Guid id = AnsiConsole.Ask<Guid>("Enter [green]account id[/]?");
            var accountWindow = new AccountWindow(_handler, _bank, id);
            accountWindow.Run();
        }

        private void GoToClient()
        {
            Guid id = AnsiConsole.Ask<Guid>("Enter [green]client id[/]?");
            var clientWindow = new ClientWindow(_handler, id, _bank);
            clientWindow.Run();
        }

        private void ShowAccounts()
        {
            Table table = TableGenerator.GenerateTable(_handler.GetAccounts(_bank));
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }

        private void ShowClients()
        {
            Table table = TableGenerator.GenerateTable(_handler.GetClients(_bank));
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }

        private void AddClient()
        {
            var builder = new ClientBuilder();
            string name = AnsiConsole.Ask<string>("Enter [green]client name[/]?");
            string surname = AnsiConsole.Ask<string>("Enter [green]client surname[/]?");
            builder.SetName(name, surname);

            List<string> additionalInfo = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("What additional info you would like to provide")
                    .NotRequired()
                    .PageSize(10)
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle, " + "[green]<enter>[/] to accept)[/]")
                    .AddChoices("Address", "Passport"));
            foreach (string info in additionalInfo)
            {
                switch (info)
                {
                    case "Address":
                        string address = AnsiConsole.Ask<string>("Enter [green]client address[/]?");
                        builder.SetAddress(address);
                        break;
                    case "Passport":
                        string passport = AnsiConsole.Ask<string>("Enter [green]client passport[/]?");
                        builder.SetId(passport);
                        break;
                }
            }

            _handler.AddClient(_bank, builder);
            AnsiConsole.WriteLine($"[green]Client added[/]");
            Prompts.ReturnPrompt();
        }
    }
}