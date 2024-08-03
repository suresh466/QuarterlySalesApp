using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuarterlySalesApp.Migrations
{
    public partial class ChangeSeedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "DOB",
                value: new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "DOB",
                value: new DateTime(1956, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
