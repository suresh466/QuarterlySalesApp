using System;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySalesApp.Models
{
    public class DateNotBeforeAttribute : ValidationAttribute
    {
        private readonly DateTime _date;

        public DateNotBeforeAttribute(string dateString)
        {
            _date = DateTime.Parse(dateString);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            if (date < _date)
            {
                return new ValidationResult(ErrorMessage ?? $"Date must not be before {_date.ToShortDateString()}.");
            }
            return ValidationResult.Success;
        }
    }
}