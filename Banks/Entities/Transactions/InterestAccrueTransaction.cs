using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class InterestAccrueTransaction : AccrueTransaction
    {
        public InterestAccrueTransaction(decimal amount, AbstractAccount account, DateTime time)
            : base(amount, account, time)
        {
        }

        internal InterestAccrueTransaction()
        {
        }

        public override void Cancel()
        {
            throw new InvalidOperationException("Interest accrue transaction can not be canceled");
        }
    }
}