using QuarterlySalesApp.Models;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySales.Models
{
    public class ManagerNotSelfAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var employee = (Employee)validationContext.ObjectInstance;

            if (employee.EmployeeId == employee.ManagerId)
            {
                return new ValidationResult(ErrorMessage ?? "An employee cannot be their own manager.");
            }

            return ValidationResult.Success;
        }
    }
}