using Banks.Database;
using Banks.Entities;
using Banks.Providers;
using Banks.Ui;
using Banks.Ui.SpectreConsole;

namespace Banks
{
    internal static class Program
    {
        private static void Main()
        {
            var dateTimeProvider = new DateTimeProvider();
            var context = new BanksContext(dateTimeProvider, "/Users/bill/Desktop/db.db");
            var repositopry = new DatabaseRepository(context);
            var centralBank = new CentralBank(repositopry);
            UiRunner.Run(dateTimeProvider, centralBank);
        }
    }
}
