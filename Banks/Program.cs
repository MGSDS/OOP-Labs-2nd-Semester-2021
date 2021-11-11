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
            var dateTimeProvider = new RealDateTimeProvider();
            var context = new BanksContext(dateTimeProvider, "path_to_database");
            var repositopry = new DatabaseRepository(context);
            var centralBank = new CentralBank(repositopry);
            UiRunner.Run(dateTimeProvider, centralBank);
        }
    }
}
