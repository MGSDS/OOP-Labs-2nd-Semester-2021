using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class WithdrawTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public WithdrawTransaction(decimal amount, AbstractAccount @from, DateTime time)
        : base(amount, time, from, null)
        {
            _errorMessage = string.Empty;
        }

        internal WithdrawTransaction()
        {
        }

        public override string ErrorMessage { get => _errorMessage; internal init => _errorMessage = value; }

        public override void Execute()
        {
            if (Status is not TransactionStatus.Ready)
                throw new InvalidOperationException("Transaction is already done");
            try
            {
                From.DecreaseBalance(Amount);
                Status = TransactionStatus.Successful;
            }
            catch (InvalidOperationException e)
            {
                _errorMessage = e.Message;
                Status = TransactionStatus.Failed;
            }
        }

        internal override void Cancel()
        {
            if (Status is not TransactionStatus.Successful)
                throw new InvalidOperationException("Transaction can not be canceled");
            From.IncreaseBalance(Amount);
            Status = TransactionStatus.Canceled;
        }
    }
}