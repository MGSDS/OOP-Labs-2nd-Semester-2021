using Banks.Database;
using Banks.Entities;
using Banks.Providers;
using Banks.Ui;
using Microsoft.EntityFrameworkCore;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite($@"DataSource=Path_To_Database;");
            var dateTimeProvider = new RealDateTimeProvider();
            var context = new BanksContext(optionsBuilder.Options, dateTimeProvider);
            var repositopry = new DatabaseRepository(context);
            var centralBank = new CentralBank(repositopry);
            UiRunner.Run(dateTimeProvider, centralBank);
        }
    }
}
