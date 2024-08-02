using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuarterlySalesApp.Migrations
{
    public partial class AdditionalSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Ramu", "Singh" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "DOB", "DateOfHire", "FirstName", "LastName", "ManagerId" },
                values: new object[] { 2, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1995, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shyam", "Singh", 1 });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SalesId", "Amount", "EmployeeId", "Quarter", "Year" },
                values: new object[,]
                {
                    { 1, 1000m, 1, 1, 2020 },
                    { 2, 2000m, 1, 2, 2020 }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SalesId", "Amount", "EmployeeId", "Quarter", "Year" },
                values: new object[] { 3, 3000m, 2, 3, 2020 });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "SalesId", "Amount", "EmployeeId", "Quarter", "Year" },
                values: new object[] { 4, 4000m, 2, 4, 2020 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "SalesId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                columns: new[] { "FirstName", "LastName" },
                values: new object[] { "Ada", "Lovelace" });
        }
    }
}
