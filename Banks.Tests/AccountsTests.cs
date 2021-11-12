using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Builders;
using Banks.Database;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using NUnit.Framework;

namespace Banks.Tests
{
    public class AccountsTests
    {
        private FakeDateTimeProvider _fakeDateTimeProvider;
        private CentralBank _centralBank;
        private DatabaseRepository _databaseRepository;

        [SetUp]
        public void Setup()
        {
            _fakeDateTimeProvider = new FakeDateTimeProvider();
            var context = new BanksContext(_fakeDateTimeProvider, "database.db");
            _databaseRepository = new DatabaseRepository(context);
            _centralBank = new CentralBank(_databaseRepository);
            _centralBank.RegisterBank(
                new Bank(_centralBank.TransactionsService,
                _fakeDateTimeProvider,
                new CreditInfoProvider(100, 10),
                new DebitInterestProvider(2),
                new DepositInterestProvider(
                    new List<Interest>
                    {
                        new Interest(5, 0),
                        new Interest(10, 100),
                    }),
                new UnverifiedLimitProvider(10),
                "Bank0"));
            var client = new Client("surname", "name");
            _centralBank.RegisterClient(client, _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0"));
        }
        
        [TearDown]
        public void TearDown()
        {
            _databaseRepository.Context.Database.EnsureDeleted();
            _databaseRepository.Context.Dispose();
        }
        
        [Test]
        [TestCase(100)]
        public void DebitAccountPerecentageAccrue_BalanceChanged(decimal amount)
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount account = _centralBank.AddDebitAccount(client, bank);
            _centralBank.Accrue(account, bank, amount);
            _fakeDateTimeProvider.Now = _fakeDateTimeProvider.Now.AddYears(1);
            _centralBank.NotifyBanks();
            Assert.AreEqual(amount * (1 + bank.DebitInterestProvider.Percentage / 100),decimal.Round(account.Balance, 3));
        }
        
        [Test]
        [TestCase(100)]
        [TestCase(95)]
        public void DepositAccountPerecentageAccrue_BalanceChanged(decimal amount)
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount account = _centralBank.AddDepositAccount(client, bank, DateTime.Now);
            _centralBank.Accrue(account, bank, amount);
            _fakeDateTimeProvider.Now = _fakeDateTimeProvider.Now.AddYears(1);
            _centralBank.NotifyBanks();
            decimal balance = amount;
            for (int i = 0; i < 365; ++i)
                balance += bank.DepositInterestProvider.GetMultiplier(balance) * balance;
            Assert.AreEqual(decimal.Round(balance,3 ),decimal.Round(account.Balance, 3));
        }
        
        [Test]
        public void CreditAccountCommissionHold_BalanceDecrease()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount account = _centralBank.AddCreditAccount(client, bank);
            _centralBank.NotifyBanks();
            Assert.AreEqual(0,decimal.Round(account.Balance, 3));
            _centralBank.Withdraw(account, bank, 5);
            _centralBank.NotifyBanks();
            Assert.AreEqual(-15,decimal.Round(account.Balance, 3));
        }
        
        [Test]
        [TestCase(1000,500)]
        [TestCase(10000,100)]
        public void UnverifiedLimitExcess_InvalidOperationException(decimal startBalance, decimal withdraw)
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount creditAccount = _centralBank.AddCreditAccount(client, bank);
            AbstractAccount debitAaccount = _centralBank.AddDebitAccount(client, bank);
            AbstractAccount depositAccount = _centralBank.AddDepositAccount(client, bank, _fakeDateTimeProvider.Now);
            AbstractTransaction transaction = creditAccount.CreateAccrueTransaction(startBalance);
            transaction.Execute();
            transaction = debitAaccount.CreateAccrueTransaction(startBalance);
            transaction.Execute();
            transaction = depositAccount.CreateAccrueTransaction(startBalance);
            transaction.Execute();
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(creditAccount, bank, withdraw));
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(debitAaccount, bank, withdraw));
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(depositAccount, bank, withdraw));
            Assert.Throws<InvalidOperationException>(() => _centralBank.Transfer(creditAccount, debitAaccount, bank, withdraw));
            Assert.Throws<InvalidOperationException>(() => _centralBank.Transfer(depositAccount, debitAaccount, bank, withdraw));
            Assert.Throws<InvalidOperationException>(() => _centralBank.Transfer(debitAaccount, creditAccount, bank, withdraw));
            Assert.AreEqual(startBalance, debitAaccount.Balance);
            Assert.AreEqual(startBalance, creditAccount.Balance);
            Assert.AreEqual(startBalance, depositAccount.Balance);
        }

        [Test]
        public void CreditLimitExceeded_InvalidOperationException()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            var editor = new ClientEditor(client);
            editor.ChangeAddress("address");
            editor.ChangePassport("passport");
            _centralBank.EditClient(editor);
            AbstractAccount creditAccount = _centralBank.AddCreditAccount(client, bank);
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(creditAccount, bank, 101));
        }

        [Test]
        public void DebitBelowZero_InvalidOperationException()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount debitAccount = _centralBank.AddDebitAccount(client, bank);
            var editor = new ClientEditor(client);
            editor.ChangeAddress("address");
            editor.ChangePassport("passport");
            _centralBank.EditClient(editor);
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(debitAccount, bank, 101));
        }
        
        [Test]
        public void DepositBelowZero_InvalidOperationException()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount depositAccount = _centralBank.AddDepositAccount(client, bank, _fakeDateTimeProvider.Now);
            var editor = new ClientEditor(client);
            editor.ChangeAddress("address");
            editor.ChangePassport("passport");
            _centralBank.EditClient(editor);
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(depositAccount, bank, 1));
        }
        
        [Test]
        public void DepositWithdrawBeforeEndDate_InvalidOperationException()
        {
            Bank bank = _centralBank.Banks.FirstOrDefault(x => x.Name == "Bank0");
            Client client = _centralBank.GetClients(bank).First();
            AbstractAccount depositAccount = _centralBank.AddDepositAccount(client, bank, _fakeDateTimeProvider.Now.AddDays(1));
            var editor = new ClientEditor(client);
            editor.ChangeAddress("address");
            editor.ChangePassport("passport");
            _centralBank.EditClient(editor);
            _centralBank.Accrue(depositAccount, bank, 1000);
            Assert.Throws<InvalidOperationException>(() => _centralBank.Withdraw(depositAccount, bank, 100));
        }
    }
}