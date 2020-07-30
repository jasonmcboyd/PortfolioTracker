namespace PortfolioTracker.EF
{
    using InteractivePreGeneratedViews;
    using System.Data.Entity;

    public partial class PortfolioTrackerContext : DbContext
    {
        public PortfolioTrackerContext()
            : base("name=PortfolioTrackerContext")
        {
            InteractiveViews.SetViewCacheFactory(this, new FileViewCacheFactory("./DbContextViews.xml"));
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Position> Positions { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Account>()
                .Property(e => e.Balance)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Positions)
                .WithRequired(e => e.Account)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Transactions)
                .WithRequired(e => e.Account)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Account>()
                .HasMany(e => e.Transfers)
                .WithRequired(e => e.Account)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Position>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Position>()
                .Property(e => e.CostBasis)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Transaction>()
                .Property(e => e.Price)
                .HasPrecision(18, 4);

            modelBuilder.Entity<Transfer>()
                .Property(e => e.Amount)
                .HasPrecision(18, 4);
        }
    }
}
