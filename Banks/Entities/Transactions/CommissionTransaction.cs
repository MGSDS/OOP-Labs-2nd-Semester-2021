using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class CommissionTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public CommissionTransaction(decimal amount, CreditAccount @from, DateTime time)
        : base(amount, time, from, null)
        {
            _errorMessage = string.Empty;
        }

        internal CommissionTransaction()
        {
        }

        public override string ErrorMessage { get => _errorMessage; internal init => _errorMessage = value; }

        public override void Cancel()
        {
            throw new InvalidOperationException("Interest accrue transaction can not be canceled");
        }

        public override void Execute()
        {
            if (Status is not TransactionStatus.Ready)
                throw new InvalidOperationException("Transaction is already done");
            From.DecreaseBalanceWithoutLimit(Amount);
            Status = TransactionStatus.Successful;
        }
    }
}