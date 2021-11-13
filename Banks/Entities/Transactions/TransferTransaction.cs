using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class TransferTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public TransferTransaction(decimal amount, AbstractAccount @from, AbstractAccount to, DateTime time)
        : base(amount, time, from, to)
        {
            _errorMessage = string.Empty;
        }

        internal TransferTransaction()
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
            }
            catch (InvalidOperationException e)
            {
                Status = TransactionStatus.Failed;
                _errorMessage = e.Message;
                return;
            }

            To.IncreaseBalance(Amount);
            Status = TransactionStatus.Successful;
        }

        internal override void Cancel()
        {
            if (Status is not TransactionStatus.Successful)
                throw new InvalidOperationException("Transaction is already canceled");
            To.DecreaseBalanceWithoutLimit(Amount);
            From.IncreaseBalance(Amount);
            _errorMessage = string.Empty;
            Status = TransactionStatus.Canceled;
        }
    }
}