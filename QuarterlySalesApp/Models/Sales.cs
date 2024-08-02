namespace QuarterlySalesApp.Models
{
    public class Sales
    {
        public int SalesId { get; set; }
        public int Quarter { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
