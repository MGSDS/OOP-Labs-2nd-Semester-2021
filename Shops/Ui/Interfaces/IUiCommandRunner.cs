using System;
using Shops.Ui.DataTypes;

namespace Shops.Ui.Interfaces
{
    public interface IUiCommandRunner
    {
        public void ShowBuyerProducts();
        public void ShowShops();
        public void RegisterProduct();
        public void RegisterShop();
        public void ShowBuyerMoney();
        public void AddBuyerMoney();
        public void ShowRegisteredProducts();
        public void ShowShopProducts();
        public void AddProductToShop();
        public void FindCheapestShop();
        public void Buy();
        public void ChangePrice();
    }
}