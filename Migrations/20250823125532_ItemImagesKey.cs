using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P26.Migrations
{
    /// <inheritdoc />
    public partial class ItemImagesKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemImages",
                table: "ItemImages");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ItemImages",
                newName: "ItemId");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ItemImages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemImages",
                table: "ItemImages",
                columns: new[] { "ItemId", "ImageUrl" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemImages",
                table: "ItemImages");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "ItemImages",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ItemImages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemImages",
                table: "ItemImages",
                column: "Id");
        }
    }
}
