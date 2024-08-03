using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuarterlySalesApp.Models
{
    // Employee model class
    public class Employee
    {
        // *Id properties are primary keys by convention and are required automatically
        public int EmployeeId { get; set; }

        // Required attribute ensures the property is not null or empty
        [Required(ErrorMessage = "First name is required.")]
        // StringLength attribute sets the maximum length of the property
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        // DataType attribute specifies the type of data for the property
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Date of hire is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Hire")]
        // DateNotBefore attribute ensures the date is not before 1995
        [DateNotBefore("1/1/1995", ErrorMessage = "Date of hire cannot be before company founding (1/1/1995).")]
        public DateTime DateOfHire { get; set; }

        [Display(Name = "Manager")]
        // ForeignKey attribute specifies the foreign key property
        public int? ManagerId { get; set; }

        // manager property is a navigation property
        public Employee Manager { get; set; }
        // managed employees property is a collection navigation property
        public List<Employee> ManagedEmployees { get; set; }
        // Sales property is a collection navigation property
        public List<Sales> Sales { get; set; }
        
        // for convenience, a read-only property that returns the full name
        public string FullName => $"{FirstName} {LastName}";
    }
}