using QuarterlySalesApp.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuarterlySales.Models
{
    public class UniqueEmployeeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var employee = (Employee)validationContext.ObjectInstance;
            var context = (QuarterlySalesContext)validationContext.GetService(typeof(QuarterlySalesContext));

            var existingEmployee = context.Employees.FirstOrDefault(e =>
                e.FirstName == employee.FirstName &&
                e.LastName == employee.LastName &&
                e.DOB == employee.DOB &&
                e.EmployeeId != employee.EmployeeId);

            if (existingEmployee != null)
            {
                return new ValidationResult($"{employee.FirstName} {employee.LastName} (DOB: {employee.DOB:d}) is already in the database.");
            }

            return ValidationResult.Success;
        }
    }
}