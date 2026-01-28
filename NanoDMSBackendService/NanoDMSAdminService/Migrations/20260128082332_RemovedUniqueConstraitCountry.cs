using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniqueConstraitCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Countries_Iso2",
                table: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Countries_Iso3",
                table: "Countries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso2",
                table: "Countries",
                column: "Iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso3",
                table: "Countries",
                column: "Iso3",
                unique: true);
        }
    }
}
