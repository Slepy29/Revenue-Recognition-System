﻿using Microsoft.EntityFrameworkCore;
using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Data;

public class DatabaseContext : DbContext
{
    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Software> Softwares { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SoftwareVersion> SoftwareVersions { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Category entity
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Category");
            e.HasKey(c => c.CategoryId);
            e.Property(c => c.Name).IsRequired();
            e.HasMany(c => c.Softwares)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasData(
                new Category { CategoryId = 1, Name = "Business" },
                new Category { CategoryId = 2, Name = "Education" }
            );
        });

        // Configure Version entity
        modelBuilder.Entity<SoftwareVersion>(e =>
        {
            e.ToTable("SoftwareVersion");
            e.HasKey(v => v.VersionId);
            e.Property(v => v.VersionNumber).IsRequired();
            e.Property(v => v.ReleaseDate).IsRequired();
            
            e.HasData(
                new SoftwareVersion { VersionId = 1, VersionNumber = "1.0", ReleaseDate = new DateTime(2023, 1, 1) },
                new SoftwareVersion { VersionId = 2, VersionNumber = "2.0", ReleaseDate = new DateTime(2024, 1, 1) }
            );
        });

        // Configure Software entity
        modelBuilder.Entity<Software>(e =>
        {
            e.ToTable("Software");
            e.HasKey(s => s.SoftwareId);
            e.Property(s => s.Name).IsRequired();
            e.Property(s => s.Description).IsRequired();
            e.HasOne(s => s.CurrentVersion)
                .WithMany(v => v.Softwares)
                .HasForeignKey(s => s.CurrentVersionId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.Category)
                .WithMany(c => c.Softwares)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(s => s.Transactions)
                .WithOne(t => t.Software)
                .HasForeignKey(t => t.SoftwareId);
            e.HasMany(s => s.Contracts)
                .WithOne(c => c.Software)
                .HasForeignKey(c => c.SoftwareId);
            e.HasMany(s => s.Discounts)
                .WithMany(d => d.Softwares)
                .UsingEntity<Dictionary<string, object>>(
                    "SoftwareDiscount",
                    j => j.HasOne<Discount>().WithMany().HasForeignKey("DiscountId"),
                    j => j.HasOne<Software>().WithMany().HasForeignKey("SoftwareId")
                );
            
            e.HasData(
                new Software { SoftwareId = 1, Name = "Business Suite", Description = "A suite of business tools", CurrentVersionId = 1, CategoryId = 1 },
                new Software { SoftwareId = 2, Name = "Educational Platform", Description = "An online education platform", CurrentVersionId = 2, CategoryId = 2 }
            );
        });

        // Configure Client entity
        modelBuilder.Entity<Client>(e =>
        {
            e.ToTable("Client");
            e.HasKey(c => c.ClientId);
            e.Property(c => c.Name);
            e.Property(c => c.Email);
            e.Property(c => c.PhoneNumber);
            e.Property(c => c.IsCompany).IsRequired();
            e.Property(c => c.PESEL).HasMaxLength(11);
            e.Property(c => c.KRS).HasMaxLength(10);
            
            e.HasMany(c => c.Transactions)
                .WithOne(t => t.Client)
                .HasForeignKey(t => t.ClientId);
            e.HasMany(c => c.Payments)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.Contracts)
                .WithOne(con => con.Client)
                .HasForeignKey(con => con.ClientId);
            
            e.HasData(
                new Client { ClientId = 1, Name = "John Doe", Email = "john.doe@example.com", PhoneNumber = "123456789", IsCompany = false, PESEL = "12345678900"},
                new Client { ClientId = 2, Name = "Tech Corp", Email = "contact@techcorp.com", PhoneNumber = "987654321", IsCompany = true, KRS = "1234567890" }
            );
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transaction");
            e.HasKey(t => t.TransactionId);
            e.HasOne(t => t.Software)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(t => t.Client)
                .WithMany(cli => cli.Transactions)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Discount entity
        modelBuilder.Entity<Discount>(e =>
        {
            e.ToTable("Discount");
            e.HasKey(d => d.DiscountId);
            e.Property(d => d.Name).IsRequired();
            e.Property(d => d.Description).IsRequired();
            e.Property(d => d.Percentage).IsRequired();
            e.Property(d => d.StartDate).IsRequired();
            e.Property(d => d.EndDate).IsRequired();
            
            e.HasData(
                new Discount { DiscountId = 1, Name = "New Year Sale", Description = "10% off", Percentage = 10, StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 1, 31) }
            );
        });

        // Configure Contract entity
        modelBuilder.Entity<Contract>(e =>
        {
            e.ToTable("Contract");
            e.HasKey(c => c.ContractId);
            e.Property(c => c.StartDate).IsRequired();
            e.Property(c => c.EndDate).IsRequired();
            e.Property(c => c.Price).IsRequired();
            e.Property(c => c.IsSigned).IsRequired();
            e.Property(c => c.AdditionalYearsOfSupport).IsRequired();

            e.HasOne(c => c.Software)
                .WithMany(s => s.Contracts)
                .HasForeignKey(c => c.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(c => c.Discount)
                .WithMany()
                .HasForeignKey(c => c.DiscountId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.Client)
                .WithMany(cli => cli.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
            
            e.HasData(
                new Contract { ContractId = 1, StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2024, 6, 30), Price = 10000, IsSigned = false, AdditionalYearsOfSupport = 1, SoftwareId = 1, ClientId = 1, DiscountId = 1 }
            );
        });

        // Configure Payment entity
        modelBuilder.Entity<Payment>(e =>
        {
            e.ToTable("Payment");
            e.HasKey(p => p.PaymentId);
            e.Property(p => p.Amount).IsRequired();
            e.Property(p => p.PaymentDate).IsRequired();

            e.HasOne(p => p.Contract)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ContractId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(p => p.Client)
                .WithMany(cli => cli.Payments)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasData(
                new Payment { PaymentId = 1, Amount = 5000, PaymentDate = new DateTime(2024, 6, 15), ContractId = 1, ClientId = 1 }
            );
        });
    }
}