using System.Collections.Generic;
using System.Linq;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Banks.Tests
{
    public class TransactionsTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private CentralBank _centralBank;
        private DatabaseRepository _repository;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new FakeDateTimeProvider();
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite($@"DataSource=database.db;");
            var context = new BanksContext(optionsBuilder.Options ,_dateTimeProvider);
            _repository = new DatabaseRepository(context);
            _centralBank = new CentralBank(_repository);
            _centralBank.RegisterBank(new Bank(_centralBank.TransactionsService,
                _dateTimeProvider,
                new CreditInfoProvider(10000, 10),
                new DebitInterestProvider(2),
                new DepositInterestProvider(
                    new List<Interest>
                    {
                        new Interest(3, 0),
                        new Interest(4, 100),
                    }),
                new UnverifiedLimitProvider(10000),
                "Bank0"));
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            _centralBank.RegisterClient(new Client("name", "surname"), bank);
        }
        [TearDown]
        public void TearDown()
        {
            _repository.Context.Database.EnsureDeleted();
            _repository.Context.Dispose();
        }

        [Test]
        public void AccrueTransactionCancel_TransactionCanceled()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            DebitAccount debit = _centralBank.AddDebitAccount(client, bank);
            _centralBank.Accrue(debit, bank, 1000);
            AbstractTransaction transaction = _centralBank.GetTransactions(debit).First();
            _centralBank.TransactionsService.Cancel(transaction);
            Assert.AreEqual(0, debit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }
        [Test]
        public void WithdrawTransactionCancel_TransactionCanceled()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            DebitAccount debit = _centralBank.AddDebitAccount(client, bank);
            _centralBank.Accrue(debit, bank, 1000);
            _centralBank.Withdraw(debit, bank, 1000);
            AbstractTransaction transaction = _centralBank.GetTransactions(debit).Last();
            _centralBank.TransactionsService.Cancel(transaction);
            Assert.AreEqual(1000, debit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }

        [Test]
        public void TransferTransactionCancel_TransactionCanceled()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            DebitAccount debit = _centralBank.AddDebitAccount(client, bank);
            CreditAccount credit = _centralBank.AddCreditAccount(client, bank);
            _centralBank.Transfer(credit, debit, bank, 100);
            AbstractTransaction transaction = _centralBank.GetTransactions(debit).First();
            _centralBank.TransactionsService.Cancel(transaction);
            Assert.AreEqual(0, debit.Balance);
            Assert.AreEqual(0, credit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }
    }
}