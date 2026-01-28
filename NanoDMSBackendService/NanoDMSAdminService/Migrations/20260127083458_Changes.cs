using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is_Virtual",
                table: "Card_Bins",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Card_Types_Name",
                table: "Card_Types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Card_Levels_Name",
                table: "Card_Levels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Card_Brands_Name",
                table: "Card_Brands",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Card_Types_Name",
                table: "Card_Types");

            migrationBuilder.DropIndex(
                name: "IX_Card_Levels_Name",
                table: "Card_Levels");

            migrationBuilder.DropIndex(
                name: "IX_Card_Brands_Name",
                table: "Card_Brands");

            migrationBuilder.DropColumn(
                name: "Is_Virtual",
                table: "Card_Bins");
        }
    }
}
