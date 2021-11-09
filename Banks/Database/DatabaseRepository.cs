using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Builders;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Services;
using Microsoft.EntityFrameworkCore;

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
        public IReadOnlyList<Client> Clients => Context.Set<Client>().ToList();
        public IReadOnlyList<AbstractAccount> Accounts => Context.Set<AbstractAccount>().ToList();
        public IReadOnlyList<AbstractTransaction> Transactions => Context.Transactions
             .Include(x => x.From)
             .Include(x => x.To).ToList();

        public void RegisterBank(Bank bank)
        {
            Context.Banks.Add(bank);
            Context.SaveChanges();
        }

        public AbstractAccount FindAccount(Guid id)
        {
            return Context.Set<AbstractAccount>().FirstOrDefault(x => x.Id == id);
        }

        public Client FindClient(Guid id)
        {
            return Context.Set<Client>().FirstOrDefault(x => x.Id == id);
        }

        public AbstractTransaction FindTransaction(Guid id)
        {
            return Context.Transactions.FirstOrDefault(x => x.Id == id);
        }

        public Bank FindBank(Guid id)
        {
            return Context.Banks.FirstOrDefault(x => x.Id == id);
        }

        public IReadOnlyList<AbstractAccount> GetBankAccounts(Bank bank)
        {
            return Context.Banks.Where(x => x == bank)
                .Include(x => x.Accounts)
                .ThenInclude(x => x.Client)
                .FirstOrDefault()
                ?.Accounts.ToList();
        }

        public IReadOnlyList<Client> GetBankClients(Bank bank)
        {
            return Context.Banks.Where(x => x == bank)
                .Include(x => x.Clients)
                .FirstOrDefault()
                ?.Clients.ToList();
        }

        public void RegisterClient(Client client, Bank bank)
        {
            Bank found = Context.Banks.Where(x => x == bank)
                .Include(x => x.Clients)
                .FirstOrDefault() ?? throw new NullReferenceException("Bank not registered");
            found.AddClient(client);
            Context.SaveChanges();
        }

        public DepositAccount AddDepositAccount(Client client, Bank bank, DateTime endDate)
        {
            Bank found = Context.Banks.Where(x => x == bank)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.UnverifiedLimitProvider)
                .Include(x => x.Accounts)
                .FirstOrDefault() ?? throw new NullReferenceException("Bank not registered");
            DepositAccount account = found.AddDepositAccount(client, endDate);
            Context.SaveChanges();
            return account;
        }

        public DebitAccount AddDebitAccount(Client client, Bank bank)
        {
            Bank found = Context.Banks.Where(x => x == bank)
                .Include(x => x.Accounts)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.UnverifiedLimitProvider)
                .FirstOrDefault() ?? throw new NullReferenceException("Bank not registered");
            DebitAccount account = found.AddDebitAccount(client);
            Context.SaveChanges();
            return account;
        }

        public CreditAccount AddCreditAccount(Client client, Bank bank)
        {
            Bank found = Context.Banks.Where(x => x == bank)
                .Include(x => x.Accounts)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.UnverifiedLimitProvider)
                .FirstOrDefault() ?? throw new NullReferenceException("Bank not registered");
            CreditAccount account = found.AddCreditAccount(client);
            Context.SaveChanges();
            return account;
        }

        public void Accrue(AbstractAccount account, Bank bank, decimal money)
        {
            Bank foundBank = Context.Banks
                .FirstOrDefault(x => x == bank) ?? throw new NullReferenceException("Bank not registered");
            AbstractAccount foundAccount = Context.Set<AbstractAccount>()
                .Include(x => x.Client)
                .Include(x => x.UnverifiedLimit)
                .Include(x => (x as CreditAccount).CreditInfo)
                .FirstOrDefault(x => x == account) ?? throw new NullReferenceException("Account not registered");
            foundBank.Accrue(foundAccount, money);
            Context.SaveChanges();
        }

        public void Withdraw(AbstractAccount account, Bank bank, decimal money)
        {
            Bank foundBank = Context.Banks
                .FirstOrDefault(x => x == bank) ?? throw new NullReferenceException("Bank not registered");
            AbstractAccount foundAccount = Context.Set<AbstractAccount>()
                .Include(x => x.Client)
                .Include(x => x.UnverifiedLimit)
                .Include(x => (x as CreditAccount).CreditInfo)
                .FirstOrDefault(x => x == account) ?? throw new NullReferenceException("Account not registered");
            foundBank.Withdraw(foundAccount, money);
            Context.SaveChanges();
        }

        public void Transfer(AbstractAccount from, AbstractAccount to, Bank bank, decimal money)
        {
            Bank foundBank = Context.Banks
                .FirstOrDefault(x => x == bank) ?? throw new NullReferenceException("Bank not registered");
            AbstractAccount foundFrom = Context.Set<AbstractAccount>()
                .Include(x => x.Client)
                .Include(x => x.UnverifiedLimit)
                .Include(x => (x as CreditAccount).CreditInfo)
                .FirstOrDefault(x => x == from) ?? throw new NullReferenceException("Account not registered");
            AbstractAccount foundTo = Context.Set<AbstractAccount>()
                .Include(x => x.Client)
                .Include(x => x.UnverifiedLimit)
                .Include(x => (x as CreditAccount).CreditInfo)
                .FirstOrDefault(x => x == to) ?? throw new NullReferenceException("Account not registered");
            foundBank.Transfer(foundFrom, foundTo, money);
            Context.SaveChanges();
        }

        public void CancelTransaction(AbstractTransaction transaction)
        {
            AbstractTransaction found = Context.Transactions.FirstOrDefault(x => x == transaction) ??
                                         throw new InvalidOperationException("Transaction not found");
            found.Cancel();
            Context.SaveChanges();
        }

        public void AddTransaction(AbstractTransaction transaction)
        {
            Context.Transactions.Add(transaction);
            Context.SaveChanges();
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Client client)
        {
            return Context.Set<AbstractAccount>().Where(x => x.Client == client).ToList();
        }

        public IReadOnlyList<AbstractTransaction> GetAccountTransactions(AbstractAccount account)
        {
            return Context.Transactions.Where(x => x.From == account || x.To == account).ToList();
        }

        public void NotifyBanks()
        {
            var banks = Context.Banks.Include(x => x.Accounts).ToList();
            foreach (Bank bank in banks)
                bank.AccountsUpdate();
            Context.SaveChanges();
        }

        public void SubscribeClient(Client client, Bank bank)
        {
            Bank foundBank = Context.Banks
                .Include(x => Clients)
                .Include(x => x.SubscribedClients)
                .FirstOrDefault(x => x == bank) ?? throw new NullReferenceException("Bank not registered");
            foundBank.Subscribe(Context.Set<Client>().FirstOrDefault(x => x == client)
                                ?? throw new NullReferenceException("Client not registered"));
            Context.SaveChanges();
        }

        public void ChangeTerms(
            Bank bank,
            CreditInfoProvider creditInfoProvider,
            DebitInterestProvider debitInterestProvider,
            DepositInterestProvider depositInterestProvider,
            UnverifiedLimitProvider unverifiedLimit)
        {
            Bank found = Context.Banks.Where(x => x == bank)
                .Include(x => x.CreditInfoProvider)
                .Include(x => x.DebitInterestProvider)
                .Include(x => x.DepositInterestProvider)
                .Include(x => x.UnverifiedLimitProvider)
                .Include(x => x.SubscribedClients)
                .FirstOrDefault() ?? throw new NullReferenceException("No such bank registered");
            found.ChangeTerms(creditInfoProvider, debitInterestProvider, depositInterestProvider, unverifiedLimit);
            Context.SaveChanges();
        }

        public void EditClient(ClientEditor editor)
        {
            editor.ApplyChanges();
            Context.SaveChanges();
        }
    }
}