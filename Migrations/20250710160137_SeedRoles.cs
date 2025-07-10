using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP_P26.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "Description" },
                values: new object[,]
                {
                    { "Administrator", true, true, true, true, "Системний адміністратор" },
                    { "Employee", true, false, true, false, "Співробітник компанії" },
                    { "Moderator", false, true, true, true, "Редактор контенту" },
                    { "SelfRegistered", false, false, false, false, "Самостійно зареєстрований користувач" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Administrator");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Employee");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "Moderator");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: "SelfRegistered");
        }
    }
}
