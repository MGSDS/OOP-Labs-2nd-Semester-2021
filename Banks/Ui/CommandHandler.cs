using System;
using System.Collections.Generic;
using Banks.Builders;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Ui.Interfaces;

namespace Banks.Ui
{
    public class CommandHandler : ICommandHandler
    {
        private readonly CentralBank _centralBank;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CommandHandler(CentralBank centralBank, DateTimeProvider dateTimeProvider)
        {
            _centralBank = centralBank;
            _dateTimeProvider = dateTimeProvider;
        }

        public IReadOnlyList<Bank> GetBanks()
        {
            return _centralBank.Banks;
        }

        public Bank GetBank(Guid bankId)
        {
            return _centralBank.FindBank(bankId);
        }

        public IReadOnlyList<AbstractAccount> GetAccounts(Bank bank)
        {
            return _centralBank.GetAccounts(bank);
        }

        public IReadOnlyList<Client> GetClients(Bank bank)
        {
            return _centralBank.GetClients(bank);
        }

        public IReadOnlyList<AbstractTransaction> GetTransactions()
        {
            return _centralBank.TransactionsService.Transactions;
        }

        public IReadOnlyList<AbstractTransaction> GetAccountTransactions(Guid accountId)
        {
            AbstractAccount account = _centralBank.FindAccount(accountId);
            return _centralBank.GetTransactions(account);
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Guid clientId)
        {
            Client client = _centralBank.FindClient(clientId);
            return _centralBank.GetAccounts(client);
        }

        public void CancelTransaction(Guid transactionId)
        {
            AbstractTransaction transaction = _centralBank.FindTransaction(transactionId);
            _centralBank.TransactionsService.Cancel(transaction);
        }

        public Guid RegisterBank(
            string name,
            decimal unverifiedLimit,
            decimal creditLimit,
            decimal creditCommission,
            decimal debitInterest,
            List<Interest> depositInterests)
        {
            var bank = new Bank(
                _centralBank.TransactionsService,
                _dateTimeProvider,
                new CreditInfoProvider(creditLimit, creditCommission),
                new DebitInterestProvider(debitInterest),
                new DepositInterestProvider(depositInterests),
                new UnverifiedLimitProvider(unverifiedLimit),
                name);
            _centralBank.RegisterBank(bank);
            return bank.Id;
        }

        public Guid AddClient(Bank bank, Client client)
        {
            _centralBank.RegisterClient(client, bank);
            return client.Id;
        }

        public void AddClient(Bank bank, ClientBuilder builder)
        {
            _centralBank.RegisterClient(builder, bank);
        }

        public Client GetClient(Guid clientId)
        {
            return _centralBank.FindClient(clientId);
        }

        public Guid CreateCreditAccount(Client client, Bank bank)
        {
            CreditAccount account = _centralBank.AddCreditAccount(client, bank);
            return account.Id;
        }

        public Guid CreateDebitAccount(Client client, Bank bank)
        {
            DebitAccount account = _centralBank.AddDebitAccount(client, bank);
            return account.Id;
        }

        public Guid CreateDepositAccount(Client client, Bank bank, DateTime endDate)
        {
            DepositAccount account = _centralBank.AddDepositAccount(client, bank, endDate);
            return account.Id;
        }

        public AbstractAccount GetAccount(Guid accountId)
        {
            return _centralBank.FindAccount(accountId);
        }

        public void Accrue(Bank bank, AbstractAccount account, decimal amount)
        {
            _centralBank.Accrue(account, bank, amount);
        }

        public void Withdraw(Bank bank, AbstractAccount account, decimal amount)
        {
             _centralBank.Withdraw(account, bank, amount);
        }

        public void Transfer(AbstractAccount account, Bank bank, Guid destinationAccountId, decimal amount)
        {
            _centralBank.Transfer(account, _centralBank.FindAccount(destinationAccountId), bank, amount);
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Client client)
        {
            return _centralBank.GetAccounts(client);
        }
    }
}