using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSRightsService.Migrations
{
    /// <inheritdoc />
    public partial class RightsInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Route = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Parent_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessLocation_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_Parent_Id",
                        column: x => x.Parent_Id,
                        principalTable: "Menus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Role_Audit_Logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Entity = table.Column<string>(type: "text", nullable: false),
                    Old_Value = table.Column<string>(type: "text", nullable: true),
                    New_Value = table.Column<string>(type: "text", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessLocation_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Audit_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role_Menu_Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Role_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Menu_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Permissions = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessLocation_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Is_Active = table.Column<bool>(type: "boolean", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Menu_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Menu_Permissions_Menus_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Code",
                table: "Menus",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Parent_Id",
                table: "Menus",
                column: "Parent_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Menu_Permissions_Menu_Id",
                table: "Role_Menu_Permissions",
                column: "Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Menu_Permissions_Role_Id_Menu_Id",
                table: "Role_Menu_Permissions",
                columns: new[] { "Role_Id", "Menu_Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Role_Audit_Logs");

            migrationBuilder.DropTable(
                name: "Role_Menu_Permissions");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
