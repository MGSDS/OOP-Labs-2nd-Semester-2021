using System;
using System.Collections.Generic;
using Banks.Database;
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

        public IReadOnlyList<Bank> Banks => DatabaseRepository.Banks;
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

        public AbstractTransaction GetTransaction(Guid transactionId)
        {
            return DatabaseRepository.GetTransaction(transactionId);
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