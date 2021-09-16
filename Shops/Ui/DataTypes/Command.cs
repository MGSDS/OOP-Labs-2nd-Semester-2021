using System;

namespace Shops.Ui.DataTypes
{
    public class Command
    {
        public Command(string name, Action action)
        {
            Action = action ?? throw new ArgumentException("Action could not be null");
            Name = name;
        }

        public string Name { get; }
        public Action Action { get; }
        public override string ToString() => Name;
    }
}