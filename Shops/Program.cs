using Shops.Entities;
using Shops.Services;
using Spectre.Console;

namespace Shops
{
    internal class Program
    {
        public static void Main()
        {
            var ui = new Ui(new ShopService(), new Buyer(1000));
            ui.Run();
        }
    }
}
