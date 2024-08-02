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
                return new ValidationResult(ErrorMessage ?? "Sales data already exists for this employee, year, and quarter.");
            }

            return ValidationResult.Success;
        }
    }
}