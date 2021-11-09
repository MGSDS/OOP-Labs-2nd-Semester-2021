using System.Collections.Generic;
using System.Linq;
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
        private DatabaseRepository _databaseRepository;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new DateTimeProvider();
            var context = new BanksContext(_dateTimeProvider, "database.db");
            _databaseRepository = new DatabaseRepository(context);
            _centralBank = new CentralBank(_databaseRepository);
        }
        
        [TearDown]
        public void TearDown()
        {
            _databaseRepository.Context.Database.EnsureDeleted();
            _databaseRepository.Context.Dispose();
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
            Bank result = _centralBank.Banks.FirstOrDefault(x => x == bank);
            Assert.AreEqual(bank, result);
        }
    }
}