using System;
using System.Security.Cryptography;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class AccrueTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public AccrueTransaction(decimal amount, AbstractAccount account, DateTime time)
            : base(amount, time)
        {
            Account = account;
            _errorMessage = string.Empty;
        }

        internal AccrueTransaction()
        {
        }

        public AbstractAccount Account { get; init; }

        public override string ErrorMessage { get => _errorMessage; internal init => _errorMessage = value; }

        public override void Execute()
        {
            if (Status is not TransactionStatus.Ready)
                throw new InvalidOperationException("Transaction is already done");
            try
            {
                Account.IncreaseBalance(Amount);
                Status = TransactionStatus.Successful;
            }
            catch (InvalidOperationException e)
            {
                Status = TransactionStatus.Failed;
                _errorMessage = e.Message;
            }
        }

        public override void Cancel()
        {
            if (Status is not TransactionStatus.Successful or TransactionStatus.CancelationFailed)
                throw new InvalidOperationException("Transaction can not be canceled");
            try
            {
                Account.DecreaseBalanceWithoutLimit(Amount);
                _errorMessage = string.Empty;
                Status = TransactionStatus.Canceled;
            }
            catch (InvalidOperationException e)
            {
                _errorMessage = e.Message;
                Status = TransactionStatus.CancelationFailed;
            }
        }
    }
}