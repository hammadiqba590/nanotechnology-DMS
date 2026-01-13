using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSBusinessService.Migrations
{
    /// <inheritdoc />
    public partial class BusinessInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Time_Zone_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Financial_Year_Start_Month = table.Column<Guid>(type: "uuid", nullable: false),
                    Stock_Accounting_Method = table.Column<Guid>(type: "uuid", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: false),
                    Ntn = table.Column<string>(type: "text", nullable: false),
                    Stn = table.Column<string>(type: "text", nullable: false),
                    Tax3 = table.Column<string>(type: "text", nullable: false),
                    Tax4 = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Business_Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<Guid>(type: "uuid", nullable: false),
                    Postal_Code = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Mobile = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Website = table.Column<string>(type: "text", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_Location_Business_Business_Id",
                        column: x => x.Business_Id,
                        principalTable: "Business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_User_Business_Business_Id",
                        column: x => x.Business_Id,
                        principalTable: "Business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_Config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name_Key = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Config_Value = table.Column<string>(type: "text", nullable: false),
                    Config_Type = table.Column<string>(type: "text", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Location_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_Config_Business_Business_Id",
                        column: x => x.Business_Id,
                        principalTable: "Business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_Config_Business_Location_Business_Location_Id",
                        column: x => x.Business_Location_Id,
                        principalTable: "Business_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Business_Location_User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Location_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Location_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Business_Location_User_Business_Business_Id",
                        column: x => x.Business_Id,
                        principalTable: "Business",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Business_Location_User_Business_Location_Business_Location_~",
                        column: x => x.Business_Location_Id,
                        principalTable: "Business_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Business_Config_Business_Id",
                table: "Business_Config",
                column: "Business_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Config_Business_Location_Id",
                table: "Business_Config",
                column: "Business_Location_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Location_Business_Id",
                table: "Business_Location",
                column: "Business_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Location_User_Business_Id",
                table: "Business_Location_User",
                column: "Business_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_Location_User_Business_Location_Id",
                table: "Business_Location_User",
                column: "Business_Location_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Business_User_Business_Id",
                table: "Business_User",
                column: "Business_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Business_Config");

            migrationBuilder.DropTable(
                name: "Business_Location_User");

            migrationBuilder.DropTable(
                name: "Business_User");

            migrationBuilder.DropTable(
                name: "Business_Location");

            migrationBuilder.DropTable(
                name: "Business");
        }
    }
}
