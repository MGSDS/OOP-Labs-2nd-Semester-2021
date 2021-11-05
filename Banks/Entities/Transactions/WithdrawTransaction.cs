using System;
using Banks.Entities.Accounts;

namespace Banks.Entities.Transactions
{
    public class WithdrawTransaction : AbstractTransaction
    {
        private string _errorMessage;

        public WithdrawTransaction(decimal amount, AbstractAccount account, DateTime time)
        : base(amount, time)
        {
            Account = account;
            _errorMessage = string.Empty;
        }

        internal WithdrawTransaction()
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
                Account.DecreaseBalance(Amount);
                Status = TransactionStatus.Successful;
            }
            catch (InvalidOperationException e)
            {
                _errorMessage = e.Message;
                Status = TransactionStatus.Failed;
            }
        }

        public override void Cancel()
        {
            if (Status is not TransactionStatus.Successful)
                throw new InvalidOperationException("Transaction can not be canceled");
            Account.IncreaseBalance(Amount);
            Status = TransactionStatus.Canceled;
        }
    }
}