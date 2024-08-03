using System;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySalesApp.Models
{
    // Custom validation attribute to ensure a date is not before a specified date
    public class DateNotBeforeAttribute : ValidationAttribute
    {
        private readonly DateTime _date;

        public DateNotBeforeAttribute(string dateString)
        {
            _date = DateTime.Parse(dateString);
        }

        // Validate the date by comparing it to the specified date
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            if (date < _date)
            {
                return new ValidationResult(ErrorMessage ?? $"Date must not be before {_date.ToShortDateString()}.");
            }
            // Return success if the date is valid
            return ValidationResult.Success;
        }
    }
}