using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Banks.Database;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Services;

namespace Banks.Entities
{
    public class Bank
    {
        private List<Client> _clients;
        private List<Client> _subscribedClients;
        private List<AbstractAccount> _accounts;

        public Bank(
            TransactionsService transactionsService,
            IDateTimeProvider dateTimeProvider,
            CreditInfoProvider creditInfoProvider,
            DebitInterestProvider debitInterestProvider,
            DepositInterestProvider depositInterestProvider,
            UnverifiedLimitProvider unverifiedLimitProvider,
            string name)
        {
            Id = Guid.NewGuid();
            Clients = new List<Client>();
            Accounts = new List<AbstractAccount>();
            DateTimeProvider = dateTimeProvider;
            CreditInfoProvider = creditInfoProvider;
            DebitInterestProvider = debitInterestProvider;
            DepositInterestProvider = depositInterestProvider;
            UnverifiedLimitProvider = unverifiedLimitProvider;
            Name = name;
            TransactionsService = transactionsService;
            _subscribedClients = new List<Client>();
        }

        internal Bank(BanksContext context)
        {
            DateTimeProvider = context.DateTimeProvider;
            TransactionsService = context.TransactionsService;
        }

        public Guid Id { get; internal init; }

        public string Name { get; internal init; }

        [NotMapped]
        public TransactionsService TransactionsService { get; internal init; }
        public IReadOnlyList<Client> Clients { get => _clients; internal init => _clients = (List<Client>)value; }
        public IReadOnlyList<Client> SubscribedClients { get => _subscribedClients; internal init => _subscribedClients = (List<Client>)value; }
        public IReadOnlyList<AbstractAccount> Accounts { get => _accounts; internal init => _accounts = (List<AbstractAccount>)value; }
        [NotMapped]
        public IDateTimeProvider DateTimeProvider { get; internal set; }
        public CreditInfoProvider CreditInfoProvider { get; internal set; }
        public DebitInterestProvider DebitInterestProvider { get; internal set; }
        public DepositInterestProvider DepositInterestProvider { get; internal set; }
        public UnverifiedLimitProvider UnverifiedLimitProvider { get; internal set; }

        internal void AddClient(Client client)
        {
            if (Clients.Any(x => x.Id == client.Id))
                throw new InvalidOperationException("Such client is already added");
            _clients.Add(client);
        }

        internal DebitAccount AddDebitAccount(Client client)
        {
            CheckClientExists(client);
            var account = new DebitAccount(client, UnverifiedLimitProvider, DateTimeProvider, DebitInterestProvider);
            _accounts.Add(account);
            return account;
        }

        internal CreditAccount AddCreditAccount(Client client)
        {
            CheckClientExists(client);
            var account = new CreditAccount(client, UnverifiedLimitProvider, DateTimeProvider, CreditInfoProvider);
            _accounts.Add(account);
            return account;
        }

        internal DepositAccount AddDepositAccount(Client client, DateTime endDate)
        {
            CheckClientExists(client);
            var account = new DepositAccount(client, UnverifiedLimitProvider, DateTimeProvider, DepositInterestProvider, endDate);
            _accounts.Add(account);
            return account;
        }

        internal AbstractTransaction Withdraw(AbstractAccount account, decimal amount)
        {
            AbstractTransaction transaction = account.Withdraw(amount);
            TransactionsService.Add(transaction);
            if (transaction.Status is not TransactionStatus.Successful)
                throw new InvalidOperationException(transaction.ErrorMessage);
            return transaction;
        }

        internal AbstractTransaction Accrue(AbstractAccount account, decimal amount)
        {
            AbstractTransaction transaction = account.Accrue(amount);
            TransactionsService.Add(transaction);
            if (transaction.Status is not TransactionStatus.Successful)
                throw new InvalidOperationException(transaction.ErrorMessage);
            return transaction;
        }

        internal AbstractTransaction Transfer(AbstractAccount from, AbstractAccount to, decimal amount)
        {
            AbstractTransaction transaction = from.Transfer(amount, to);
            TransactionsService.Add(transaction);
            if (transaction.Status is not TransactionStatus.Successful)
                throw new InvalidOperationException(transaction.ErrorMessage);
            return transaction;
        }

        internal void AccountsUpdate()
        {
            foreach (AbstractAccount account in _accounts)
            {
                AbstractTransaction transaction = account.Notify();
                if (transaction.Status is not TransactionStatus.Successful)
                    throw new InvalidOperationException(transaction.ErrorMessage);
                TransactionsService.Add(transaction);
            }
        }

        internal void Subscribe(Client client)
        {
            if (!_clients.Contains(client))
                throw new InvalidOperationException("client is not registered");
            if (SubscribedClients.Contains(client))
                throw new InvalidOperationException("Client is already subscribed");
            _subscribedClients.Add(client);
        }

        internal void ChangeTerms(
            CreditInfoProvider creditInfoProvider = null,
            DebitInterestProvider debitInterestProvider = null,
            DepositInterestProvider depositInterestProvider = null,
            UnverifiedLimitProvider unverifiedLimitProvider = null)
        {
            if (creditInfoProvider is not null)
                CreditInfoProvider = creditInfoProvider;
            if (debitInterestProvider is not null)
                DebitInterestProvider = debitInterestProvider;
            if (depositInterestProvider is not null)
                DepositInterestProvider = depositInterestProvider;
            if (unverifiedLimitProvider is not null)
                UnverifiedLimitProvider = unverifiedLimitProvider;
            foreach (Client subscribedClient in _subscribedClients)
                subscribedClient.Notify();
        }

        private void CheckClientExists(Client client)
        {
            if (!_clients.Contains(client))
                throw new InvalidOperationException("Client is not registered");
        }
    }
}