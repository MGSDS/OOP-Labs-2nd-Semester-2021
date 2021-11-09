using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class InterestAccrueTransaction : AccrueTransaction
    {
        public InterestAccrueTransaction(decimal amount, AbstractAccount from, DateTime time)
            : base(amount, from, time)
        {
        }

        internal InterestAccrueTransaction()
        {
        }

        internal override void Cancel()
        {
            throw new InvalidOperationException("Interest accrue transaction can not be canceled");
        }
    }
}