using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaxSystem.Migrations
{
    /// <inheritdoc />
    public partial class notifnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "num_read_faxes",
                table: "USERS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "num_read_faxes_branches",
                table: "USERS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 1,
                column: "RoleName",
                value: "المشرف العام");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 2,
                column: "RoleName",
                value: "صلاحية رئيس الفرع");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 4,
                column: "RoleName",
                value: "صلاحية السيد المديرر");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 5,
                column: "RoleName",
                value: "صلاحية نائب المدير");

            migrationBuilder.UpdateData(
                table: "USERS",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "num_read_faxes", "num_read_faxes_branches" },
                values: new object[] { 0, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "num_read_faxes",
                table: "USERS");

            migrationBuilder.DropColumn(
                name: "num_read_faxes_branches",
                table: "USERS");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 1,
                column: "RoleName",
                value: "Super Admin");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 2,
                column: "RoleName",
                value: "صلاحية تعديل المكاتبات");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 4,
                column: "RoleName",
                value: "صلاحية اتخاذ قرار");

            migrationBuilder.UpdateData(
                table: "ROLES",
                keyColumn: "RoleID",
                keyValue: 5,
                column: "RoleName",
                value: "صلاحية اضافة رأي");
        }
    }
}
