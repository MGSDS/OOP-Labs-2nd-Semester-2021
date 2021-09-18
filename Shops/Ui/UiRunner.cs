using Shops.Entities;
using Shops.Services;
using Shops.Ui.SpectreConsole;
using Command = Shops.Ui.DataTypes.Command;

namespace Shops.Ui
{
    public class UiRunner
    {
        private UiCommandRunner _choice;

        public UiRunner(Buyer buyer, ShopService shopService)
        {
            _choice = new UiCommandRunner(new CommandHandler(shopService, buyer));
        }

        public void Run()
        {
            bool exit = false;
            while (!exit)
            {
                exit = !_choice.ChooseAction();
            }
        }
    }
}