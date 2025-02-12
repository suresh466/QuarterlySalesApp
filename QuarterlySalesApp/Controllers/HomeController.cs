﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuarterlySalesApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuarterlySales.Models;
using System.Diagnostics;
using System.Globalization;

namespace QuarterlySalesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuarterlySalesContext _context;

        public HomeController(ILogger<HomeController> logger, QuarterlySalesContext context)
        {
            _logger = logger;
            // Inject the database context into the controller
            _context = context;
        }

        // display the list of sales records
        public async Task<IActionResult> Index(int? employeeId, int? year, int? quarter, string sortBy, string sortDirection, int pageNumber = 1, int pageSize = 4)
        {
            IQueryable<Sales> sales = _context.Sales.Include(s => s.Employee);

            // Apply filters
            if (employeeId.HasValue)
            {
                sales = sales.Where(s => s.EmployeeId == employeeId.Value);
            }
            if (year.HasValue)
            {
                sales = sales.Where(s => s.Year == year.Value);
            }
            if (quarter.HasValue)
            {
                sales = sales.Where(s => s.Quarter == quarter.Value);
            }

            // Apply sorting
            switch (sortBy)
            {
                case "employee":
                    sales = sortDirection == "asc" ? sales.OrderBy(s => s.Employee.LastName) : sales.OrderByDescending(s => s.Employee.LastName);
                    break;
                case "year":
                    sales = sortDirection == "asc" ? sales.OrderBy(s => s.Year) : sales.OrderByDescending(s => s.Year);
                    break;
                case "quarter":
                    sales = sortDirection == "asc" ? sales.OrderBy(s => s.Quarter) : sales.OrderByDescending(s => s.Quarter);
                    break;
                case "amount":
                    sales = sortDirection == "asc" ? sales.OrderBy(s => s.Amount) : sales.OrderByDescending(s => s.Amount);
                    break;
                default:
                    sales = sales.OrderByDescending(s => s.Year).ThenByDescending(s => s.Quarter);
                    break;
            }

            // Apply paging
            var totalItems = await sales.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var salesData = await sales.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            // Prepare ViewBag data for dropdowns and current state
            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.LastName).ToListAsync(), "EmployeeId", "FullName", employeeId);
            ViewBag.Years = new SelectList(await _context.Sales.Select(s => s.Year).Distinct().OrderByDescending(y => y).ToListAsync(), year);
            ViewBag.Quarters = new SelectList(new[] { 1, 2, 3, 4 }, quarter);

            ViewBag.CurrentSort = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.EmployeeId = employeeId;
            ViewBag.Year = year;
            ViewBag.Quarter = quarter;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(salesData);
        }

        // display the form to add a new employee
        [HttpGet]
        public IActionResult AddEmployee()
        {
            // Create a SelectList for the dropdown list
            ViewBag.Managers = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        // add a new employee to the database
        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            ModelState.Clear();

            // Capitalize names before saving
            employee.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(employee.FirstName.ToLower());
            employee.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(employee.LastName.ToLower());

            // Custom validations
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e =>
                e.FirstName.ToLower() == employee.FirstName.ToLower() &&
                e.LastName.ToLower() == employee.LastName.ToLower() &&
                e.DOB == employee.DOB);

            // Check if the employee already exists
            if (existingEmployee != null)
            {
                // Add a model error if the employee already exists
                ModelState.AddModelError("", $"{employee.FirstName} {employee.LastName} (DOB: {employee.DOB:d}) is already in the database.");
            }

            // Check if the employee is their own manager
            if (employee.ManagerId.HasValue && employee.ManagerId.Value != 0)
            {
                var manager = await _context.Employees.FindAsync(employee.ManagerId.Value);
                if (manager != null &&
                    manager.FirstName.ToLower() == employee.FirstName.ToLower() &&
                    manager.LastName.ToLower() == employee.LastName.ToLower() &&
                    manager.DOB == employee.DOB)
                {
                    // Add a model error if the employee is their own manager
                    ModelState.AddModelError("", "An employee cannot be their own manager.");
                }
            }

            if (ModelState.IsValid)
            {
                // Add the employee to the database if valid
                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Employee added successfully!";
                return RedirectToAction("Index");
            }

            // Create a SelectList for the dropdown list
            ViewBag.Managers = new SelectList(await _context.Employees.ToListAsync(), "EmployeeId", "FullName", employee.ManagerId);
            return View(employee);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //This is sale stuff from here

        // display the form to add a new sales data
        [HttpGet]
        public IActionResult AddSales()
        {
            // Create a SelectList for the dropdown list
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSales(Sales sale)
        {
            ModelState.Clear();

            // Custom validations
            var existingSale = await _context.Sales.FirstOrDefaultAsync(s =>
                s.EmployeeId == sale.EmployeeId &&
                s.Year == sale.Year &&
                s.Quarter == sale.Quarter);

            // Check if the sales data already exists
            if (existingSale != null)
            {
                var employee = await _context.Employees.FindAsync(sale.EmployeeId);
                ModelState.AddModelError("", $"Sales for {employee.FirstName} {employee.LastName} for {sale.Year} Q{sale.Quarter} are already in the database.");
            }

            // Check if the year is after 2000
            if (sale.Year <= 2000)
            {
                ModelState.AddModelError("", "Year must be after 2000.");
            }

            // Check if the quarter is between 1 and 4
            if (sale.Quarter < 1 || sale.Quarter > 4)
            {
                ModelState.AddModelError("", "Quarter must be between 1 and 4.");
            }
            
            // Check if the amount is greater than 0
            if (sale.Amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0.");
            }

            
            if (ModelState.IsValid)
            {
                // Add the sales data to the database if valid
                _context.Add(sale);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Sales data added successfully!";
                return RedirectToAction("Index");
            }

            // Create a SelectList for the dropdown list
            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeId", "FullName", sale.EmployeeId);
            return View(sale);
        }
    }
}