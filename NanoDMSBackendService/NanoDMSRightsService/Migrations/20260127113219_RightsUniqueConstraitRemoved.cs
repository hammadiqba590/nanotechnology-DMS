using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSRightsService.Migrations
{
    /// <inheritdoc />
    public partial class RightsUniqueConstraitRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Role_Menu_Permissions_Role_Id",
                table: "Role_Menu_Permissions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Role_Menu_Permissions_Role_Id",
                table: "Role_Menu_Permissions",
                column: "Role_Id",
                unique: true);
        }
    }
}
