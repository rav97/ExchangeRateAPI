using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

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
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

                var myConnectionString = configuration.GetConnectionString("ApplicationData");
                optionsBuilder.UseSqlServer(myConnectionString);
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
