using System;
using System.Collections.Generic;
using Banks.Database;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Services;

namespace Banks.Entities
{
    public class CentralBank
    {
        public CentralBank(DatabaseRepository databaseRepository)
        {
            DatabaseRepository = databaseRepository;
        }

        public IReadOnlyList<Bank> Banks => DatabaseRepository.GetBankFull();
        public DatabaseRepository DatabaseRepository { get; }
        public TransactionsService TransactionsService { get => DatabaseRepository.TransactionsService; }

        public void RegisterBank(Bank bank)
        {
            DatabaseRepository.RegisterBank(bank);
        }

        public Bank GetBank(Guid bankId)
        {
            return DatabaseRepository.GetBank(bankId);
        }

        public Bank GetBank(string name)
        {
            return DatabaseRepository.GetBank(name);
        }

        public IReadOnlyList<AbstractAccount> GetBankAccounts(Bank bank)
        {
            return DatabaseRepository.GetBankAccounts(bank);
        }

        public IReadOnlyList<AbstractTransaction> GetAccountTransactions(Guid accountId)
        {
            return DatabaseRepository.GetAccountTransactions(accountId);
        }

        public IReadOnlyList<AbstractAccount> GetClientAccounts(Guid userId, Guid bankId)
        {
            return DatabaseRepository.GetClientAccounts(userId, bankId);
        }

        public void NotifyBanks()
        {
            foreach (Bank bank in Banks)
                DatabaseRepository.GetBank(bank.Id).AccountsUpdate();
            DatabaseRepository.SaveChanges();
        }

        public void SaveChanges()
        {
            DatabaseRepository.SaveChanges();
        }
    }
}