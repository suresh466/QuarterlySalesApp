using Microsoft.EntityFrameworkCore;
using QuarterlySalesApp.Models;
using System;

namespace QuarterlySales.Models
{
    public class QuarterlySalesContext : DbContext
    {
        public QuarterlySalesContext(DbContextOptions<QuarterlySalesContext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Sales> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data for initial employee (Ada Lovelace)
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    FirstName = "Ada",
                    LastName = "Lovelace",
                    DOB = new DateTime(1956, 12, 10),
                    DateOfHire = new DateTime(1995, 1, 1),
                    ManagerId = null // has no manager
                }
            );

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