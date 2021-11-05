using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Banks.Builders;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Providers;
using NUnit.Framework;

namespace Banks.Tests
{
    public class BankTests
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
                new CreditInfoProvider(100, 10),
                new DebitInterestProvider(2),
                new DepositInterestProvider(
                    new List<Interest>
                    {
                        new Interest(3, 0),
                        new Interest(4, 100),
                    }),
                10,
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
        public void AddClient_ClientAdded()
        {
            var client = new Client("surname", "name");
            _centralBank.GetBank("Bank0").AddClient(client);
            _centralBank.SaveChanges();
            Bank result = _centralBank.GetBank("Bank0");
            Assert.True(result.Clients.Contains(client));
        }
        
        [Test]
        public void AddSameClient_InvalidOperationException()
        {
            var client = new Client("surname", "name");
            _centralBank.GetBank("Bank0").AddClient(client);
            _centralBank.SaveChanges();
            Assert.Throws<InvalidOperationException>(() => _centralBank.GetBank("Bank0").AddClient(client));
        }

        [Test]
        public void SubscribeClientAndChangeTerms_ClientNotified()
        {
            var client = new Client("surname", "name");
            _centralBank.GetBank("Bank0").AddClient(client);
            _centralBank.SaveChanges();
            _centralBank.GetBank("Bank0").Subscribe(client);
            _centralBank.SaveChanges();
            Assert.Throws<NotImplementedException>(() =>
                _centralBank.GetBank("Bank0").ChangeTerms(unverifiedLimit: 100));
        }
        
        [Test]
        public void createAccounts_AccountsCreated()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.GetBank("Bank0");
            bank.AddClient(client);
            CreditAccount credit = bank.AddCreditAccount(client);
            DebitAccount debit = bank.AddDebitAccount(client);
            DepositAccount deposit = bank.AddDepositAccount(client, _dateTimeProvider.Now.AddDays(1));
            _centralBank.SaveChanges();
            Assert.True(_centralBank.GetBank("Bank0").Accounts.Contains(credit));
            Assert.True(_centralBank.GetBank("Bank0").Accounts.Contains(debit));
            Assert.True(_centralBank.GetBank("Bank0").Accounts.Contains(deposit));
        }

        [Test]
        public void TransactionsTest()
        {
            var client = new Client("surname", "name");
            ClientEditor.ChangeAddress("address", client);
            ClientEditor.ChangePassport("passport", client);
            Bank bank = _centralBank.GetBank("Bank0");
            bank.AddClient(client);
            DebitAccount debit = bank.AddDebitAccount(client);
            DebitAccount debit2 = bank.AddDebitAccount(client);
            bank.Accrue(debit, 10000);
            _centralBank.SaveChanges();
            Assert.AreEqual(debit.Balance, 10000);
            bank.Withdraw(debit, 1000);
            _centralBank.SaveChanges();
            Assert.AreEqual(debit.Balance, 9000);
            _centralBank.SaveChanges();
            bank.Transfer(debit, debit2, 9000);
            _centralBank.SaveChanges();
            Assert.AreEqual(debit.Balance, 0);
            Assert.AreEqual(debit2.Balance, 9000);
        }
        
    }
}