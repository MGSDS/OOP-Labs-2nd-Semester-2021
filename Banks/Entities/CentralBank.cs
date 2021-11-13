using System;
using System.Collections.Generic;
using Banks.Builders;
using Banks.Database;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Services;

namespace Banks.Entities
{
    public class CentralBank
    {
        private DatabaseRepository _databaseRepository;
        public CentralBank(DatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public IReadOnlyList<Bank> Banks => _databaseRepository.Banks;
        public IReadOnlyList<AbstractAccount> Accounts => _databaseRepository.Accounts;
        public IReadOnlyList<Client> Clients => _databaseRepository.Clients;
        public TransactionsService TransactionsService { get => _databaseRepository.TransactionsService; }

        public void RegisterBank(Bank bank)
        {
            _databaseRepository.RegisterBank(bank);
        }

        public AbstractAccount FindAccount(Guid id)
        {
            return _databaseRepository.FindAccount(id);
        }

        public Client FindClient(Guid id)
        {
            return _databaseRepository.FindClient(id);
        }

        public Bank FindBank(Guid id)
        {
            return _databaseRepository.FindBank(id);
        }

        public AbstractTransaction FindTransaction(Guid id)
        {
            return _databaseRepository.FindTransaction(id);
        }

        public IReadOnlyList<Client> GetClients(Bank bank)
        {
            return _databaseRepository.GetBankClients(bank);
        }

        public IReadOnlyList<AbstractAccount> GetAccounts(Bank bank)
        {
            return _databaseRepository.GetBankAccounts(bank);
        }

        public IReadOnlyList<AbstractTransaction> GetTransactions(AbstractAccount account)
        {
            return _databaseRepository.GetAccountTransactions(account);
        }

        public IReadOnlyList<AbstractAccount> GetAccounts(Client client)
        {
            return _databaseRepository.GetClientAccounts(client);
        }

        public void NotifyBanks()
        {
            _databaseRepository.NotifyBanks();
        }

        public void RegisterClient(Client client, Bank bank)
        {
            _databaseRepository.RegisterClient(client, bank);
        }

        public void RegisterClient(ClientBuilder builder, Bank bank)
        {
            RegisterClient(builder.Build(), bank);
        }

        public DepositAccount AddDepositAccount(Client client, Bank bank, DateTime endDate)
        {
            return _databaseRepository.AddDepositAccount(client, bank, endDate);
        }

        public CreditAccount AddCreditAccount(Client client, Bank bank)
        {
            return _databaseRepository.AddCreditAccount(client, bank);
        }

        public DebitAccount AddDebitAccount(Client client, Bank bank)
        {
            return _databaseRepository.AddDebitAccount(client, bank);
        }

        public void Accrue(AbstractAccount account, Bank bank, decimal money)
        {
            _databaseRepository.Accrue(account, bank, money);
        }

        public void Transfer(AbstractAccount from, AbstractAccount to, Bank bank, decimal money)
        {
            _databaseRepository.Transfer(from, to, bank, money);
        }

        public void Withdraw(AbstractAccount account, Bank bank, decimal money)
        {
            _databaseRepository.Withdraw(account, bank, money);
        }

        public void Subscribe(Client client, Bank bank)
        {
            _databaseRepository.SubscribeClient(client, bank);
        }

        public void ChangeTerms(
            Bank bank,
            CreditInfoProvider creditInfoProvider = null,
            DebitInterestProvider debitInterestProvider = null,
            DepositInterestProvider depositInterestProvider = null,
            UnverifiedLimitProvider unverifiedLimit = null)
        {
            _databaseRepository.ChangeTerms(bank, creditInfoProvider, debitInterestProvider, depositInterestProvider, unverifiedLimit);
        }

        public void EditClient(ClientEditor editor)
        {
            _databaseRepository.EditClient(editor);
        }
    }
}