using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;

namespace Banks.Ui.Interfaces
{
    public interface ICommandHandler
    {
        IReadOnlyList<Bank> GetBanks();
        Bank GetBank(Guid bankId);
        IReadOnlyList<AbstractAccount> GetAccounts(Bank bank);
        IReadOnlyList<Client> GetClients(Bank bank);
        IReadOnlyList<AbstractTransaction> GetTransactions();
        IReadOnlyList<AbstractTransaction> GetAccountTransactions(Guid accountId);
        void CancelTransaction(Guid transactionId);
        Guid RegisterBank(string name, decimal unverifiedLimit, decimal creditLimit, decimal creditCommission, decimal debitInterest, List<Interest> depositInterests);
        Guid AddClient(Bank bank, Client client);
        Client GetClient(Guid clientId, Bank bank);
        Guid CreateCreditAccount(Client client, Bank bank);
        Guid CreateDebitAccount(Client client, Bank bank);
        Guid CreateDepositAccount(Client client, Bank bank, DateTime endDate);
        IReadOnlyList<AbstractAccount> GetClientAccounts(Bank bank, Client client);
        AbstractAccount GetAccount(Bank bank, Guid accountId);
        void Accrue(Bank bank, AbstractAccount account, decimal amount);
        void Withdraw(Bank bank, AbstractAccount account, decimal amount);
        void Transfer(AbstractAccount account, Bank bank, Guid destinationBankId, Guid destinationAccountId, decimal amount);
        IReadOnlyList<AbstractAccount> GetClientAccounts(Guid userId, Guid bankId);
    }
}