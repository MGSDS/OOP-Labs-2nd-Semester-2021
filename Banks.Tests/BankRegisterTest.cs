using System.Collections.Generic;
using Banks.Database;
using Banks.Entities;
using Banks.Providers;
using NUnit.Framework;

namespace Banks.Tests
{
    public class BankRegisterTests
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
        }
        
        [TearDown]
        public void TearDown()
        {
            _centralBank.DatabaseRepository.Context.Database.EnsureDeleted();
            _centralBank.DatabaseRepository.Context.Dispose();
        }

        [Test]
        public void AddBank_ShouldAddBank()
        {
            var bank = new Bank(_centralBank.TransactionsService,
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
                "Bank");
            _centralBank.RegisterBank(bank);
            Bank result = _centralBank.GetBank(bank.Id);
            Assert.AreEqual(bank, result);
        }
    }
}