using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuarterlySalesApp.Models
{
    // Sales model class
    public class Sales
    {
        // *Id properties are primary keys by convention and are required automatically
        public int SalesId { get; set; }

        // Required attribute ensures the property is not null or empty
        [Required(ErrorMessage = "Quarter is required.")]
        // Range attribute sets the valid range of the property
        [Range(1, 4, ErrorMessage = "Quarter must be between 1 and 4.")]
        public int Quarter { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(2001, 9999, ErrorMessage = "Year must be after 2000.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        // Column attribute sets the data type and precision of the property
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        // ForeignKey attribute specifies the foreign key property
        public int EmployeeId { get; set; }

        // Employee property is a navigation property
        public Employee Employee { get; set; }
    }
}