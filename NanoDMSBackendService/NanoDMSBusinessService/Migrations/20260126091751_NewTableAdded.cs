using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSBusinessService.Migrations
{
    /// <inheritdoc />
    public partial class NewTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessLocationBankSettlement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Location_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bank_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Merchant_Share = table.Column<decimal>(type: "numeric", nullable: false),
                    Bank_Share = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax_Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax_On_Merchant = table.Column<int>(type: "integer", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLocationBankSettlement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLocationBankSettlement_Business_Location_Business_L~",
                        column: x => x.Business_Location_Id,
                        principalTable: "Business_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLocationPsp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Business_Location_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Psp_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    Create_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Create_User = table.Column<Guid>(type: "uuid", nullable: false),
                    Last_Update_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Last_Update_User = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLocationPsp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLocationPsp_Business_Location_Business_Location_Id",
                        column: x => x.Business_Location_Id,
                        principalTable: "Business_Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLocationBankSettlement_Business_Location_Id",
                table: "BusinessLocationBankSettlement",
                column: "Business_Location_Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLocationPsp_Business_Location_Id",
                table: "BusinessLocationPsp",
                column: "Business_Location_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessLocationBankSettlement");

            migrationBuilder.DropTable(
                name: "BusinessLocationPsp");
        }
    }
}
