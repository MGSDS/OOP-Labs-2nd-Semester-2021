using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Services;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Banks.Database
{
    public class DatabaseRepository
    {
        public DatabaseRepository(BanksContext context)
        {
            Context = context;
            Context.TransactionsService = new TransactionsService(this);
        }

        public BanksContext Context { get; }
        public TransactionsService TransactionsService { get => Context.TransactionsService; }
        public IDateTimeProvider DateTimeProvider { get => Context.DateTimeProvider; }
        public IReadOnlyList<Bank> Banks => Context.Banks.ToList();
        public IReadOnlyList<AbstractTransaction> Transactions => Context.Transactions
             .Include(x => x.From)
             .Include(x => x.To).ToList();

        public void RegisterBank(Bank bank)
        {
            Context.Banks.Add(bank);
            Context.SaveChanges();
        }

        public Bank GetBank(Guid bankId)
        {
            Bank bank = Context.Banks.Where(x => bankId == x.Id)
                .Include(x => x.Clients)
                .Include(x => x.Accounts)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.SubscribedClients)
                .Include(x => x.DepositInterestProvider.Interests)
                .FirstOrDefault() ?? throw new InvalidOperationException("Bank not found");
            return bank;
        }

        public Bank GetBank(string name)
        {
            Bank bank = Context.Banks.Where(x => name == x.Name)
                .Include(x => x.Clients)
                .Include(x => x.Accounts)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.SubscribedClients)
                .Include(x => x.DepositInterestProvider.Interests)
                .FirstOrDefault() ?? throw new InvalidOperationException("Bank not found");
            return bank;
        }

        public IReadOnlyList<Bank> GetBankFull()
        {
            List<Bank> banks = Context.Banks
                .Include(x => x.Clients)
                .Include(x => x.Accounts)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.SubscribedClients)
                .Include(x => x.DepositInterestProvider.Interests)
                .ToList();
            return banks;
        }

        public void CancelTransaction(Guid transactionId)
        {
            AbstractTransaction transaction = Context.Transactions.Find(transactionId) ??
                                              throw new InvalidOperationException("Transaction not found");
            transaction.Cancel();
            Context.SaveChanges();
        }

        public IReadOnlyList<AbstractAccount> GetBankAccounts(Bank bank)
        {
            List<AbstractAccount> accounts = Context.Banks.Where(x => x == bank)
                .Include(x => x.Clients)
                .Include(x => x.Accounts)
                .FirstOrDefault()
                ?.Accounts.ToList();
            return accounts;
        }

        public void AddTransaction(AbstractTransaction transaction)
        {
            Context.Transactions.Add(transaction);
            Context.SaveChanges();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

        public AbstractTransaction GetTransaction(Guid transactionId)
        {
            AbstractTransaction transaction = Context.Transactions.Find(transactionId) ??
                                               throw new InvalidOperationException("Transaction not found");
            return transaction;
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Guid userId, Guid bankId)
        {
            return Context.Banks.Where(x => x.Id == bankId)
                .Include(x => x.Accounts)
                .SelectMany(x => x.Accounts)
                .Include(x => x.Client)
                .Where(x => x.Client.Id == userId)
                .ToList();
        }

        public IReadOnlyList<AbstractTransaction> GetClientTransactions(Guid userId)
        {
            IReadOnlyList<AbstractTransaction> transactions = Context.Transactions.Where(x => x.From.Client.Id == userId).ToList();
            return transactions;
        }

        public IReadOnlyList<AbstractTransaction> GetAccountTransactions(Guid accountId)
        {
            IReadOnlyList<AbstractTransaction> transactions = Context.Transactions.Where(x => x.From.Id == accountId).ToList();
            return transactions;
        }
    }
}