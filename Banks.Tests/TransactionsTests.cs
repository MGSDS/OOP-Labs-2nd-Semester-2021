using System.Collections.Generic;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using NUnit.Framework;

namespace Banks.Tests
{
    public class TransactionsTests
    {
        private IDateTimeProvider _dateTimeProvider;
        private CentralBank _centralBank;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new DateTimeProvider();
            var context = new BanksContext(_dateTimeProvider, "database.db");
            var repositopry = new DatabaseRepository(context);
            _centralBank = new CentralBank(repositopry);
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
            _centralBank.SaveChanges();
        }
        [TearDown]
        public void TearDown()
        {
            _centralBank.DatabaseRepository.Context.Database.EnsureDeleted();
            _centralBank.DatabaseRepository.Context.Dispose();
        }

        [Test]
        public void AccrueTransactionCancel_TransactionCanceled()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.GetBank("Bank0");
            bank.AddClient(client);
            DebitAccount debit = bank.AddDebitAccount(client);
            AbstractTransaction transaction =  bank.Accrue(debit, 1000);
            transaction.Cancel();
            _centralBank.SaveChanges();
            Assert.AreEqual(0, debit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }
        [Test]
        public void WithdrawTransactionCancel_TransactionCanceled()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.GetBank("Bank0");
            bank.AddClient(client);
            DebitAccount debit = bank.AddDebitAccount(client);
            bank.Accrue(debit, 1000);
            AbstractTransaction transaction = bank.Withdraw(debit, 1000);
            transaction.Cancel();
            _centralBank.SaveChanges();
            Assert.AreEqual(1000, debit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }
        
        [Test]
        public void TransferTransactionCancel_TransactionCanceled()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.GetBank("Bank0");
            bank.AddClient(client);
            CreditAccount credit = bank.AddCreditAccount(client);
            DebitAccount debit = bank.AddDebitAccount(client);
            AbstractTransaction transaction = bank.Transfer(credit, debit, 500);
            transaction.Cancel();
            _centralBank.SaveChanges();
            Assert.AreEqual(0, debit.Balance);
            Assert.AreEqual(0, credit.Balance);
            Assert.AreEqual(TransactionStatus.Canceled, transaction.Status);
        }
    }
}