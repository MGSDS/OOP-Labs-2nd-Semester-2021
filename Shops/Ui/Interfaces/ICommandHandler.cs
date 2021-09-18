using System;
using System.Collections.Generic;
using Shops.Entities;
using Shops.Services;
using Shops.Ui.DataTypes;

namespace Shops.Ui.Interfaces
{
    public interface ICommandHandler
    {
        IReadOnlyList<CountableProduct> GetBuyerProducts();
        IReadOnlyList<Shop> GetShops();
        void RegisterProduct(string name);
        void RegisterShop(string name, string address);
        uint GetBuyerMoney();
        void AddBuyerMoney(uint money);
        IReadOnlyList<Product> GetRegisteredProducts();
        IReadOnlyList<SellableProduct> GetShopProducts(uint id);
        void AddProductToShop(uint shopId, string productName, uint count, uint price);
        uint FindCheapestShop(string productName, uint count);
        void Buy(uint id, string productName, uint count);
        void ChangePrice(uint id, string productName, uint price);
        uint GetProductPrice(uint shopId, string productName);
    }
}