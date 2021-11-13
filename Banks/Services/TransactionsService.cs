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

        public void Cancel(AbstractTransaction transaction)
        {
            _databaseRepository.CancelTransaction(transaction);
        }

        internal void Add(AbstractTransaction transaction)
        {
            _databaseRepository.AddTransaction(transaction);
        }
    }
}