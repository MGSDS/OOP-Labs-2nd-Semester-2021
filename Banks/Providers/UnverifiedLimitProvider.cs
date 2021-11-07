using System;
using System.Security.Cryptography.X509Certificates;

namespace Banks.Providers
{
    public class UnverifiedLimitProvider
    {
        public UnverifiedLimitProvider(decimal limit)
        {
            UnverifiedLimit = limit;
         }

        internal UnverifiedLimitProvider()
        {
        }

        public Guid Id { get; init; }
        public decimal UnverifiedLimit { get; set; }
    }
}