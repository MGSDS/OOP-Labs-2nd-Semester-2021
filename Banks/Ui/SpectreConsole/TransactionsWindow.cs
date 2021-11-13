using System;
using System.Collections.Generic;
using Banks.Ui.Commands;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class TransactionsWindow
    {
        private ICommandHandler _handler;
        private List<Command> _commands;
        private bool _run;

        public TransactionsWindow(ICommandHandler handler)
        {
            _handler = handler;
            _run = false;
            _commands = new List<Command>(new[]
            {
                new Command("Show transactions", new Action(ShowTransactions)),
                new Command("Show account transactions", new Action(ShowAccountTransactions)),
                new Command("Cancel transaction", new Action(CancelTransaction)),
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

        private void ShowTransactions()
        {
            Table table = TableGenerator.GenerateTable(_handler.GetTransactions());
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }

        private void ShowAccountTransactions()
        {
            Guid accountId = Prompts.AskGuid("account id");
            Table table = TableGenerator.GenerateTable(_handler.GetAccountTransactions(accountId));
            AnsiConsole.Write(table);
            Prompts.ReturnPrompt();
        }

        private void CancelTransaction()
        {
            Guid transactionId = Prompts.AskGuid("transaction id");
            _handler.CancelTransaction(transactionId);
        }
    }
}