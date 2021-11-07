using Banks.Entities;
using Banks.Providers;
using Banks.Ui.SpectreConsole;

namespace Banks.Ui
{
    public static class UiRunner
    {
        public static void Run(DateTimeProvider dateTimeProvider, CentralBank centralBank)
        {
            var ui = new MainWindow(new CommandHandler(centralBank, dateTimeProvider));
            ui.Run();
        }
    }
}