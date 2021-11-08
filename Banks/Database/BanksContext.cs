using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Providers;
using Banks.Services;
using Microsoft.EntityFrameworkCore;

namespace Banks.Database
{
    public class BanksContext : DbContext
    {
        public BanksContext(IDateTimeProvider dateTimeProvider, string pathToDatabase)
        {
            DateTimeProvider = dateTimeProvider;
            PathToDatabase = pathToDatabase;
            Database.EnsureCreated();
        }

        public string PathToDatabase { get; }
        public TransactionsService TransactionsService { get; set; }
        public IDateTimeProvider DateTimeProvider { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<AbstractTransaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($@"DataSource={PathToDatabase};");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditAccount>();
            modelBuilder.Entity<DebitAccount>();
            modelBuilder.Entity<DepositAccount>();
            modelBuilder.Entity<AccrueTransaction>();
            modelBuilder.Entity<CommissionTransaction>();
            modelBuilder.Entity<InterestAccrueTransaction>();
            modelBuilder.Entity<TransferTransaction>();
            modelBuilder.Entity<WithdrawTransaction>();
            base.OnModelCreating(modelBuilder);
        }
    }
}