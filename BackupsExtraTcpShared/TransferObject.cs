using System;
using Newtonsoft.Json;

namespace BackupsExtraTcpShared
{
    public class TransferObject
    {
        public TransferObject(object? obj)
        {
            Object = obj;
            Type = obj.GetType();
        }

        [JsonConstructor]

        private TransferObject(object? obj, Type type)
        {
            Object = obj;
            Type = type;
        }

        public object? Object { get; }
        public Type? Type { get; }
    }
}