using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuarterlySalesApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuarterlySales.Models;
using System.Diagnostics;

namespace QuarterlySalesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuarterlySalesContext _context;

        public HomeController(ILogger<HomeController> logger, QuarterlySalesContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? employeeId)
        {
            IQueryable<Sales> sales = _context.Sales.Include(s => s.Employee);

            if (employeeId.HasValue)
            {
                sales = sales.Where(s => s.EmployeeId == employeeId.Value);
            }

            var salesData = await sales
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Quarter)
                .ToListAsync();

            ViewBag.Employees = new SelectList(await _context.Employees.OrderBy(e => e.LastName).ToListAsync(), "EmployeeId", "FullName", employeeId);
            ViewBag.SelectedEmployee = employeeId;

            return View(salesData);
        }

        [HttpGet]
        public IActionResult AddEmployee()
        {
            ViewBag.Managers = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                bool hasErrors = false;

                // Check for existing employee
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e =>
                        e.FirstName == employee.FirstName &&
                        e.LastName == employee.LastName &&
                        e.DOB == employee.DOB);

                if (existingEmployee != null)
                {
                    ModelState.AddModelError("", $"{employee.FirstName} {employee.LastName} (DOB: {employee.DOB:d}) is already in the database.");
                    hasErrors = true;
                }

                // Check if employee is their own manager
                if (employee.EmployeeId == employee.ManagerId)
                {
                    ModelState.AddModelError("", $"Manager and employee can't be the same person.");
                    hasErrors = true;
                }

                // Check hire date
                if (employee.DateOfHire < new DateTime(1995, 1, 1))
                {
                    ModelState.AddModelError("", "Date of hire cannot be before company founding (1/1/1995).");
                    hasErrors = true;
                }

                if (!hasErrors)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Employee added successfully!";
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Managers = new SelectList(_context.Employees, "EmployeeId", "FullName", employee.ManagerId);
            return View(employee);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //This is sale stuff from here

        public async Task<IActionResult> Sales()
        {
            var sales = await _context.Sales
                .Include(s => s.Employee)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Quarter)
                .ToListAsync();
            return View(sales);
        }

        [HttpGet]
        public IActionResult AddSales()
        {
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSales(Sales sale)
        {
            if (ModelState.IsValid)
            {
                // Additional server-side validation
                var existingSale = await _context.Sales
                    .FirstOrDefaultAsync(s => s.EmployeeId == sale.EmployeeId && s.Year == sale.Year && s.Quarter == sale.Quarter);

                if (existingSale != null)
                {
                    ModelState.AddModelError("", "Sales data already exists for this employee, year, and quarter.");
                }
                else
                {
                    _context.Add(sale);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Sales data added successfully!";
                    return RedirectToAction("Sales");
                }
            }
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeId", "FullName", sale.EmployeeId);
            return View(sale);
        }
    }
}