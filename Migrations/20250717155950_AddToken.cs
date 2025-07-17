using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_P26.Migrations
{
    /// <inheritdoc />
    public partial class AddToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    Jti = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Sub = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Iat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nbf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aud = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iss = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.Jti);
                    table.ForeignKey(
                        name: "FK_AccessTokens_UserAccesses_Sub",
                        column: x => x.Sub,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_Sub",
                table: "AccessTokens",
                column: "Sub");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");
        }
    }
}
