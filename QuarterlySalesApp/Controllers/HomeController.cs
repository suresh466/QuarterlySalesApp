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
            ModelState.Clear();

            // Capitalize names before saving
            employee.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(employee.FirstName.ToLower());
            employee.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(employee.LastName.ToLower());

            // Custom validations
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(e =>
                e.FirstName.ToLower() == employee.FirstName.ToLower() &&
                e.LastName.ToLower() == employee.LastName.ToLower() &&
                e.DOB == employee.DOB);

            if (existingEmployee != null)
            {
                ModelState.AddModelError("", $"{employee.FirstName} {employee.LastName} (DOB: {employee.DOB:d}) is already in the database.");
            }

            if (employee.ManagerId.HasValue && employee.ManagerId.Value != 0)
            {
                var manager = await _context.Employees.FindAsync(employee.ManagerId.Value);
                if (manager != null &&
                    manager.FirstName.ToLower() == employee.FirstName.ToLower() &&
                    manager.LastName.ToLower() == employee.LastName.ToLower() &&
                    manager.DOB == employee.DOB)
                {
                    ModelState.AddModelError("", "An employee cannot be their own manager.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Employee added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Managers = new SelectList(await _context.Employees.ToListAsync(), "EmployeeId", "FullName", employee.ManagerId);
            return View(employee);
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
            ModelState.Clear();

            // Custom validations
            var existingSale = await _context.Sales.FirstOrDefaultAsync(s =>
                s.EmployeeId == sale.EmployeeId &&
                s.Year == sale.Year &&
                s.Quarter == sale.Quarter);

            if (existingSale != null)
            {
                var employee = await _context.Employees.FindAsync(sale.EmployeeId);
                ModelState.AddModelError("", $"Sales for {employee.FirstName} {employee.LastName} for {sale.Year} Q{sale.Quarter} are already in the database.");
            }

            if (sale.Year <= 2000)
            {
                ModelState.AddModelError("", "Year must be after 2000.");
            }

            if (sale.Quarter < 1 || sale.Quarter > 4)
            {
                ModelState.AddModelError("", "Quarter must be between 1 and 4.");
            }

            if (sale.Amount <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(sale);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Sales data added successfully!";
                return RedirectToAction("Sales");
            }

            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeId", "FullName", sale.EmployeeId);
            return View(sale);
        }
    }
}