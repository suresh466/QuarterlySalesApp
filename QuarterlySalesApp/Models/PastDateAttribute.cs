using System;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySales.Models
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime)value;
            if (date > DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "Date must be in the past.");
            }
            return ValidationResult.Success;
        }
    }
}