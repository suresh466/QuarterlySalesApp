using Microsoft.EntityFrameworkCore;
using QuarterlySalesApp.Models;
using System;

namespace QuarterlySales.Models
{
    // Database context class for the Quarterly Sales application
    public class QuarterlySalesContext : DbContext
    {
        public QuarterlySalesContext(DbContextOptions<QuarterlySalesContext> options)
            : base(options)
        { }

        // DbSet properties for database tables so they can be queried and saved
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Sales> Sales { get; set; }

        // Override the OnModelCreating method to configure the database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for initial employee (Ada Lovelace)
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    FirstName = "Ramu",
                    LastName = "Singh",
                    DOB = new DateTime(1960, 1, 1),
                    DateOfHire = new DateTime(1995, 1, 1),
                    ManagerId = null
                },
                new Employee
                {
                    EmployeeId = 2,
                    FirstName = "Shyam",
                    LastName = "Singh",
                    DOB = new DateTime(1970, 1, 1),
                    DateOfHire = new DateTime(1995, 1, 1),
                    ManagerId = 1 // Ram Singh is Shyam Singh's manager
                });
            // seed data for initial sales
            modelBuilder.Entity<Sales>().HasData(
                new Sales
                {
                    SalesId = 1,
                    Quarter = 1,
                    Year = 2020,
                    Amount = 1000,
                    EmployeeId = 1
                },
                new Sales
                {
                    SalesId = 2,
                    Quarter = 2,
                    Year = 2020,
                    Amount = 2000,
                    EmployeeId = 1
                },
                new Sales
                {
                    SalesId = 3,
                    Quarter = 3,
                    Year = 2020,
                    Amount = 3000,
                    EmployeeId = 2,
                },
                new Sales
                {
                    SalesId = 4,
                    Quarter = 4,
                    Year = 2020,
                    Amount = 4000,
                    EmployeeId = 2
                });

            // Configure relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(m => m.ManagedEmployees)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sales>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure constraints
            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.FirstName, e.LastName, e.DOB })
                .IsUnique();

            modelBuilder.Entity<Sales>()
                .HasIndex(s => new { s.Quarter, s.Year, s.EmployeeId })
                .IsUnique();
        }
    }
}