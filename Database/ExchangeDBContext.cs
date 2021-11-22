using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ExchangeRateAPI.Database
{
    public partial class ExchangeDBContext : DbContext
    {
        public ExchangeDBContext()
        {
        }

        public ExchangeDBContext(DbContextOptions<ExchangeDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiKey> ApiKeys { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LAPTOPRAFALA;Initial Catalog=ExchangeDB;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.ApiKey1)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("api_key");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ValidTo)
                    .HasColumnType("datetime")
                    .HasColumnName("valid_to");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(3)
                    .HasColumnName("currency_code");

                entity.Property(e => e.CurrencyName)
                    .HasMaxLength(150)
                    .HasColumnName("currency_name");
            });

            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CurrencyFrom)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("currency_from");

                entity.Property(e => e.CurrencyTo)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("currency_to");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date");

                entity.Property(e => e.ExchangeRate1)
                    .HasColumnType("decimal(19, 4)")
                    .HasColumnName("exchange_rate");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
