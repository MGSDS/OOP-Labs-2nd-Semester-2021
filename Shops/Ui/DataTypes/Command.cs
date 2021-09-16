using System;

namespace Shops.Ui.DataTypes
{
    public record Command(string name, Action cmd)
    {
        public override string ToString() => name;
    }
}