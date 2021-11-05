using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Builders;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Providers;
using NUnit.Framework;

namespace Banks.Tests
{
    public class AccountsTests
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
                        new Interest(5, 0),
                        new Interest(10, 100),
                    }),
                10,
                "Bank0"));
            var client = new Client("surname", "name");
            _centralBank.GetBank("Bank0").AddClient(client);
            _centralBank.SaveChanges();
        }
        
        [TearDown]
        public void TearDown()
        {
            _centralBank.DatabaseRepository.Context.Database.EnsureDeleted();
            _centralBank.DatabaseRepository.Context.Dispose();
        }
        
        [Test]
        [TestCase(100)]
        public void DebitAccountPerecentageAccrue_BalanceChanged(decimal amount)
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            AbstractAccount account = bank.AddDebitAccount(client);
            bank.Accrue(account, amount);
            _centralBank.SaveChanges();
            _dateTimeProvider.Now = _dateTimeProvider.Now.AddYears(1);
            _centralBank.NotifyBanks();
            _centralBank.SaveChanges();
            Assert.AreEqual(amount * (1 + bank.DebitInterestProvider.Percentage / 100),decimal.Round(account.Balance, 3));
        }
        
        [Test]
        [TestCase(100)]
        [TestCase(95)]
        public void DepositAccountPerecentageAccrue_BalanceChanged(decimal amount)
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            AbstractAccount account = bank.AddDepositAccount(client, _dateTimeProvider.Now);
            bank.Accrue(account, amount);
            _centralBank.SaveChanges();
            _dateTimeProvider.Now = _dateTimeProvider.Now.AddYears(1);
            _centralBank.NotifyBanks();
            _centralBank.SaveChanges();
            decimal balance = amount;
            for (int i = 0; i < 365; ++i)
                balance += bank.DepositInterestProvider.GetMultiplier(balance) * balance;
            Assert.AreEqual(decimal.Round(balance,3 ),decimal.Round(account.Balance, 3));
        }
        
        [Test]
        public void CreditAccountCommissionHold_BalanceDecrease()
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            AbstractAccount account = bank.AddCreditAccount(client);
            _centralBank.SaveChanges();
            _centralBank.NotifyBanks();
            Assert.AreEqual(0,decimal.Round(account.Balance, 3));
            bank.Withdraw(account, 5);
            _centralBank.NotifyBanks();
            _centralBank.SaveChanges();
            Assert.AreEqual(-15,decimal.Round(account.Balance, 3));
        }
        
        [Test]
        [TestCase(1000,500)]
        [TestCase(10000,100)]
        public void UnverifiedLimitExcess_InvalidOperationException(decimal startBalance, decimal withdraw)
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            AbstractAccount creditAccount = bank.AddCreditAccount(client);
            AbstractAccount debitAaccount = bank.AddDebitAccount(client);
            AbstractAccount depositAccount = bank.AddDepositAccount(client, _dateTimeProvider.Now);
            creditAccount.Accrue(startBalance);
            debitAaccount.Accrue(startBalance);
            depositAccount.Accrue(startBalance);
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(creditAccount, withdraw));
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(debitAaccount, withdraw));
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(depositAccount, withdraw));
            Assert.Throws<InvalidOperationException>(() => bank.Transfer(creditAccount, debitAaccount, withdraw));
            Assert.Throws<InvalidOperationException>(() => bank.Transfer(depositAccount, debitAaccount, withdraw));
            Assert.Throws<InvalidOperationException>(() => bank.Transfer(debitAaccount, creditAccount, withdraw));
            _centralBank.SaveChanges();
            Assert.AreEqual(startBalance, debitAaccount.Balance);
            Assert.AreEqual(startBalance, creditAccount.Balance);
            Assert.AreEqual(startBalance, depositAccount.Balance);
        }

        [Test]
        public void CreditLimitExceeded_InvalidOperationException()
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            ClientEditor.ChangePassport("passport", client);
            ClientEditor.ChangeAddress("addres", client);
            AbstractAccount creditAccount = bank.AddCreditAccount(client);
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(creditAccount, 101));
            _centralBank.SaveChanges();
        }

        [Test]
        public void DebitBelowZero_InvalidOperationException()
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            ClientEditor.ChangePassport("passport", client);
            ClientEditor.ChangeAddress("addres", client);
            AbstractAccount debitAccount = bank.AddDebitAccount(client);
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(debitAccount, 101));
            _centralBank.SaveChanges();
        }
        
        [Test]
        public void DepositBelowZero_InvalidOperationException()
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            ClientEditor.ChangePassport("passport", client);
            ClientEditor.ChangeAddress("addres", client);
            AbstractAccount depositAccount = bank.AddDepositAccount(client, _dateTimeProvider.Now);
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(depositAccount, 1));
            _centralBank.SaveChanges();
        }
        
        [Test]
        public void DepositWithdrawBeforeEndDate_InvalidOperationException()
        {
            Bank bank = _centralBank.GetBank("Bank0");
            Client client = bank.Clients.First();
            ClientEditor.ChangePassport("passport", client);
            ClientEditor.ChangeAddress("addres", client);
            AbstractAccount depositAccount = bank.AddDepositAccount(client, _dateTimeProvider.Now.AddYears(5));
            bank.Accrue(depositAccount, 1000);
            Assert.Throws<InvalidOperationException>(() => bank.Withdraw(depositAccount, 100));
            _centralBank.SaveChanges();
        }
    }
}