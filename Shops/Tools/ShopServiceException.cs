using System;
using System.Runtime.Intrinsics.Arm;

namespace Shops.Tools
{
    public class ShopServiceException : Exception
    {
        public ShopServiceException(string message)
            : base(message) { }
    }
}