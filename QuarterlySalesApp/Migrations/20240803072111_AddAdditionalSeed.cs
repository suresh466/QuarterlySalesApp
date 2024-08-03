using Microsoft.EntityFrameworkCore.Migrations;

namespace QuarterlySalesApp.Migrations
{
    public partial class AddAdditionalSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SalesId", "Amount", "EmployeeId", "Quarter", "Year" },
                values: new object[] { 5, 40000m, 2, 2, 2021 });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SalesId", "Amount", "EmployeeId", "Quarter", "Year" },
                values: new object[] { 6, 50000m, 1, 3, 2021 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 6);
        }
    }
}
