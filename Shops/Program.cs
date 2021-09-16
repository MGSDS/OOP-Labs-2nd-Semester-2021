using Shops.Entities;
using Shops.Services;
using Shops.Ui;

namespace Shops
{
    internal class Program
    {
        private static void Main()
        {
            uint money = 1000;
            var ui = new UiRunner(new Buyer(money), new ShopService());
            ui.Run();
        }
    }
}
