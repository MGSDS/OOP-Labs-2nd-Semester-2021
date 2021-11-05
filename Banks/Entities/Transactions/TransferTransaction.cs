using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class TransferTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public TransferTransaction(decimal amount, AbstractAccount from, AbstractAccount to, DateTime time)
        : base(amount, time)
        {
            From = from;
            To = to;
            _errorMessage = string.Empty;
        }

        internal TransferTransaction()
        {
        }

        public AbstractAccount From { get; internal init; }
        public AbstractAccount To { get; internal init; }

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

            try
            {
                To.IncreaseBalance(Amount);
                Status = TransactionStatus.Successful;
            }
            catch (InvalidOperationException e)
            {
                From.IncreaseBalance(Amount);
                _errorMessage = e.Message;
                Status = TransactionStatus.Failed;
            }
        }

        public override void Cancel()
        {
            if (Status is not (TransactionStatus.Successful or TransactionStatus.CancelationFailed))
                throw new InvalidOperationException("Transaction is already canceled");
            try
            {
                To.DecreaseBalance(Amount);
            }
            catch (InvalidOperationException e)
            {
                _errorMessage = e.Message;
                Status = TransactionStatus.CancelationFailed;
                return;
            }

            try
            {
                From.IncreaseBalance(Amount);
                _errorMessage = string.Empty;
                Status = TransactionStatus.Canceled;
            }
            catch (InvalidOperationException e)
            {
                _errorMessage = e.Message;
                To.IncreaseBalance(Amount);
                Status = TransactionStatus.CancelationFailed;
            }
        }
    }
}