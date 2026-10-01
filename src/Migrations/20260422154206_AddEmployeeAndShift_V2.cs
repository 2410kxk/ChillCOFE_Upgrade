using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLiQuanCaffee.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAndShift_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* BỎ QUA CỘT DESCRIPTION VÌ ĐÃ TỒN TẠI TRONG DATABASE 
               migrationBuilder.AddColumn<string>(
                   name: "Description",
                   table: "Products",
                   type: "nvarchar(max)",
                   nullable: true);
            */

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HourlyRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    ShiftID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftNumber = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.ShiftID);
                    table.ForeignKey(
                        name: "FK_Shifts_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_EmployeeID",
                table: "Shifts",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Employees");

            /* BỎ QUA KHI ROLLBACK VÌ CỘT NÀY KHÔNG ĐƯỢC TẠO RA TRONG BẢN MIGRATION NÀY
               migrationBuilder.DropColumn(
                   name: "Description",
                   table: "Products");
            */
        }
    }
}