using QuarterlySales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySalesApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [PastDate(ErrorMessage = "Date of birth must be in the past.")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Date of hire is required.")]
        [PastDate(ErrorMessage = "Date of hire must be in the past.")]
        [Display(Name = "Date of Hire")]
        [DateNotBefore("1/1/1995", ErrorMessage = "Date of hire cannot be before company founding (1/1/1995).")]
        public DateTime DateOfHire { get; set; }

        [Display(Name = "Manager")]
        [ManagerNotSelf(ErrorMessage = "An employee cannot be their own manager.")]
        public int? ManagerId { get; set; }

        public Employee Manager { get; set; }
        public List<Employee> ManagedEmployees { get; set; }
        public List<Sales> Sales { get; set; }

        [UniqueEmployee]
        public string FullName => $"{FirstName} {LastName}";
    }
}