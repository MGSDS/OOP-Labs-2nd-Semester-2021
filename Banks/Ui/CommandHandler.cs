using System;
using System.Collections.Generic;
using System.Linq;
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
            return _centralBank.GetBank(bankId);
        }

        public IReadOnlyList<AbstractAccount> GetAccounts(Bank bank)
        {
            return bank.Accounts;
        }

        public IReadOnlyList<Client> GetClients(Bank bank)
        {
            return bank.Clients;
        }

        public IReadOnlyList<AbstractTransaction> GetTransactions()
        {
            return _centralBank.TransactionsService.Transactions;
        }

        public IReadOnlyList<AbstractTransaction> GetAccountTransactions(Guid accountId)
        {
            return _centralBank.GetAccountTransactions(accountId);
        }

        public IReadOnlyList<AbstractAccount> GetUserAccounts(Guid userId, Guid bankId)
        {
            return _centralBank.GetClientAccounts(userId, bankId);
        }

        public void CancelTransaction(Guid transactionId)
        {
            _centralBank.TransactionsService.CancelTransaction(transactionId);
            _centralBank.SaveChanges();
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
            _centralBank.SaveChanges();
            return bank.Id;
        }

        public Guid AddClient(Bank bank, Client client)
        {
            bank.AddClient(client);
            _centralBank.SaveChanges();
            return client.Id;
        }

        public Client GetClient(Guid clientId, Bank bank)
        {
            return bank.Clients.FirstOrDefault(client => client.Id == clientId);
        }

        public Guid CreateCreditAccount(Client client, Bank bank)
        {
            CreditAccount account = bank.AddCreditAccount(client);
            _centralBank.SaveChanges();
            return account.Id;
        }

        public Guid CreateDebitAccount(Client client, Bank bank)
        {
            DebitAccount account = bank.AddDebitAccount(client);
            _centralBank.SaveChanges();
            return account.Id;
        }

        public Guid CreateDepositAccount(Client client, Bank bank, DateTime endDate)
        {
            DepositAccount account = bank.AddDepositAccount(client, endDate);
            _centralBank.SaveChanges();
            return account.Id;
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Bank bank, Client client)
        {
            return _centralBank.GetBankAccounts(bank).Where(x => x.Client == client).ToList();
        }

        public AbstractAccount GetAccount(Bank bank, Guid accountId)
        {
            return bank.Accounts.FirstOrDefault(account => account.Id == accountId);
        }

        public void Accrue(Bank bank, AbstractAccount account, decimal amount)
        {
            bank.Accrue(account, amount);
            _centralBank.SaveChanges();
        }

        public void Withdraw(Bank bank, AbstractAccount account, decimal amount)
        {
            bank.Withdraw(account, amount);
            _centralBank.SaveChanges();
        }

        public void Transfer(AbstractAccount account, Bank bank, Guid destinationBankId, Guid destinationAccountId, decimal amount)
        {
            bank.Transfer(account, GetBank(destinationBankId).Accounts.FirstOrDefault(x => x.Id == destinationAccountId), amount);
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Guid userId, Guid bankId)
        {
            return _centralBank.GetClientAccounts(userId, bankId);
        }
    }
}