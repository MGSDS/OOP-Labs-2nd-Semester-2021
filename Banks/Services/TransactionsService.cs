using System;
using System.Collections.Generic;
using Banks.Database;
using Banks.Entities.Transactions;

namespace Banks.Services
{
    public class TransactionsService
    {
        private DatabaseRepository _databaseRepository;
        public TransactionsService(DatabaseRepository repository)
        {
            _databaseRepository = repository;
        }

        public IReadOnlyList<AbstractTransaction> Transactions { get => _databaseRepository.Transactions; }

        public void CancelTransaction(Guid transactionId)
        {
            _databaseRepository.CancelTransaction(transactionId);
        }

        internal void AddTransaction(AbstractTransaction transaction)
        {
            _databaseRepository.AddTransaction(transaction);
        }
    }
}