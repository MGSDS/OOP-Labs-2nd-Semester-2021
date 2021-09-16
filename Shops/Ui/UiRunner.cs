using Shops.Entities;
using Shops.Services;
using Shops.Ui.SpectreConsole;
using Command = Shops.Ui.DataTypes.Command;

namespace Shops.Ui
{
    public class UiRunner
    {
        private UiChoice _choice;

        public UiRunner(Buyer buyer, ShopService shopService)
        {
            _choice = new UiChoice(new UiCommandRunner(shopService, buyer));
        }

        public void Run()
        {
            bool status = true;
            while (status)
            {
                status = _choice.Show();
            }
        }
    }
}