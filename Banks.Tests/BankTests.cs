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
        private DatabaseRepository _databaseRepository;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new FakeDateTimeProvider();
            var context = new BanksContext(_dateTimeProvider, "database.db");
            _databaseRepository = new DatabaseRepository(context);
            _centralBank = new CentralBank(_databaseRepository);
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
                new UnverifiedLimitProvider(10),
                "Bank0"));
        }
        
        [TearDown]
        public void TearDown()
        {
            _databaseRepository.Context.Database.EnsureDeleted();
            _databaseRepository.Context.Dispose();
        }

        [Test]
        public void AddClient_ClientAdded()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            _centralBank.RegisterClient(client, bank);
            Assert.True(_centralBank.GetClients(bank).Contains(client));
        }
        
        [Test]
        public void AddSameClient_InvalidOperationException()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            _centralBank.RegisterClient(client, bank);
            Assert.Throws<InvalidOperationException>(() => _centralBank.RegisterClient(client, bank));
        }

        [Test]
        public void SubscribeClientAndChangeTerms_ClientNotified()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            _centralBank.RegisterClient(client, bank);
            _centralBank.Subscribe(client, bank);
            Assert.Throws<NotImplementedException>(() => _centralBank.ChangeTerms(bank));
        }
        
        [Test]
        public void createAccounts_AccountsCreated()
        {
            var client = new Client("surname", "name");
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            _centralBank.RegisterClient(client, bank);
            AbstractAccount creditAccount = _centralBank.AddCreditAccount(client, bank);
            AbstractAccount debitAaccount = _centralBank.AddDebitAccount(client, bank);
            AbstractAccount depositAccount = _centralBank.AddDepositAccount(client, bank, _dateTimeProvider.Now.AddDays(1));
            IReadOnlyList<AbstractAccount> accounts = _centralBank.GetAccounts(bank);
            Assert.True(accounts.Contains(creditAccount));
            Assert.True(accounts.Contains(debitAaccount));
            Assert.True(accounts.Contains(depositAccount));
        }

        [Test]
        public void TransactionsTest()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            var clientBuilder = new ClientBuilder();
            clientBuilder.SetName("name", "surname");
            clientBuilder.SetAddress("address");
            clientBuilder.SetId("passport");
            Client client = clientBuilder.Build();
            _centralBank.RegisterClient(client, bank);
            DebitAccount debit = _centralBank.AddDebitAccount(client, bank);
            DebitAccount debit2 = _centralBank.AddDebitAccount(client, bank);
            _centralBank.Accrue(debit, bank, 10000);
            Assert.AreEqual(debit.Balance, 10000);
            _centralBank.Withdraw(debit, bank, 1000);
            Assert.AreEqual(debit.Balance, 9000);
            _centralBank.Transfer(debit, debit2, bank, 9000);
            Assert.AreEqual(debit.Balance, 0);
            Assert.AreEqual(debit2.Balance, 9000);
        }
        
    }
}