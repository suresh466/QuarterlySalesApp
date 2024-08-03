using QuarterlySales.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuarterlySalesApp.Models
{
    public class UniqueSalesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var sale = (Sales)validationContext.ObjectInstance;
            var context = (QuarterlySalesContext)validationContext.GetService(typeof(QuarterlySalesContext));
            var existingSale = context.Sales.FirstOrDefault(s =>
                s.EmployeeId == sale.EmployeeId &&
                s.Year == sale.Year &&
                s.Quarter == sale.Quarter &&
                s.SalesId != sale.SalesId);

            if (existingSale != null)
            {
                var employee = context.Employees.Find(sale.EmployeeId);
                return new ValidationResult($"Sales for {employee.FirstName} {employee.LastName} for {sale.Year} Q{sale.Quarter} are already in the database.");
            }
            return ValidationResult.Success;
        }
    }
}