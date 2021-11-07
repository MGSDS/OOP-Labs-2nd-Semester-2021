using System.Collections.Generic;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Ui.Interfaces;
using Spectre.Console;

namespace Banks.Ui.SpectreConsole
{
    public class TableGenerator
    {
        private ICommandHandler _handler;

        public TableGenerator(ICommandHandler handler)
        {
            _handler = handler;
        }

        public static Table GenerateTable(IReadOnlyList<Bank> banks)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Name");
            table.AddColumn("Clients count");
            table.AddColumn("Accounts count");
            foreach (Bank bank in banks)
            {
                table.AddRow(bank.Id.ToString(), bank.Name, bank.Clients.Count.ToString(), bank.Accounts.Count.ToString());
            }

            return table;
        }

        public static Table GenerateTable(IReadOnlyList<AbstractTransaction> transactions)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Type");
            table.AddColumn("Amount");
            table.AddColumn("Status");
            table.AddColumn("ErrorMessage");
            table.AddColumn("Time");
            foreach (AbstractTransaction transaction in transactions)
            {
                table.AddRow(transaction.Id.ToString(), transaction.GetType().Name, transaction.Amount.ToString(), transaction.Status.ToString(), transaction.ErrorMessage, transaction.Time.ToString());
            }

            return table;
        }

        public static Table GenerateTable(IReadOnlyList<Client> clients)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Name");
            foreach (Client client in clients)
            {
                table.AddRow(client.Id.ToString(), client.Name);
            }

            return table;
        }

        public static Table GenerateTable(IReadOnlyList<AbstractAccount> accounts)
        {
            var table = new Table();
            table.AddColumn("Id");
            table.AddColumn("Type");
            table.AddColumn("Balance");
            foreach (AbstractAccount account in accounts)
            {
                table.AddRow(account.Id.ToString(), account.GetType().Name, account.Balance.ToString());
            }

            return table;
        }
    }
}