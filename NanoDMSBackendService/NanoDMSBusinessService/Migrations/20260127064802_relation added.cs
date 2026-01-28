using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSBusinessService.Migrations
{
    /// <inheritdoc />
    public partial class relationadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLocationBankSettlement_Business_Location_Business_L~",
                table: "BusinessLocationBankSettlement");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLocationPsp_Business_Location_Business_Location_Id",
                table: "BusinessLocationPsp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessLocationPsp",
                table: "BusinessLocationPsp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessLocationBankSettlement",
                table: "BusinessLocationBankSettlement");

            migrationBuilder.RenameTable(
                name: "BusinessLocationPsp",
                newName: "BusinessLocationPsps");

            migrationBuilder.RenameTable(
                name: "BusinessLocationBankSettlement",
                newName: "BusinessLocationBankSettlements");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLocationPsp_Business_Location_Id",
                table: "BusinessLocationPsps",
                newName: "IX_BusinessLocationPsps_Business_Location_Id");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLocationBankSettlement_Business_Location_Id",
                table: "BusinessLocationBankSettlements",
                newName: "IX_BusinessLocationBankSettlements_Business_Location_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessLocationPsps",
                table: "BusinessLocationPsps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessLocationBankSettlements",
                table: "BusinessLocationBankSettlements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLocationBankSettlements_Business_Location_Business_~",
                table: "BusinessLocationBankSettlements",
                column: "Business_Location_Id",
                principalTable: "Business_Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLocationPsps_Business_Location_Business_Location_Id",
                table: "BusinessLocationPsps",
                column: "Business_Location_Id",
                principalTable: "Business_Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLocationBankSettlements_Business_Location_Business_~",
                table: "BusinessLocationBankSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLocationPsps_Business_Location_Business_Location_Id",
                table: "BusinessLocationPsps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessLocationPsps",
                table: "BusinessLocationPsps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BusinessLocationBankSettlements",
                table: "BusinessLocationBankSettlements");

            migrationBuilder.RenameTable(
                name: "BusinessLocationPsps",
                newName: "BusinessLocationPsp");

            migrationBuilder.RenameTable(
                name: "BusinessLocationBankSettlements",
                newName: "BusinessLocationBankSettlement");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLocationPsps_Business_Location_Id",
                table: "BusinessLocationPsp",
                newName: "IX_BusinessLocationPsp_Business_Location_Id");

            migrationBuilder.RenameIndex(
                name: "IX_BusinessLocationBankSettlements_Business_Location_Id",
                table: "BusinessLocationBankSettlement",
                newName: "IX_BusinessLocationBankSettlement_Business_Location_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessLocationPsp",
                table: "BusinessLocationPsp",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusinessLocationBankSettlement",
                table: "BusinessLocationBankSettlement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLocationBankSettlement_Business_Location_Business_L~",
                table: "BusinessLocationBankSettlement",
                column: "Business_Location_Id",
                principalTable: "Business_Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLocationPsp_Business_Location_Business_Location_Id",
                table: "BusinessLocationPsp",
                column: "Business_Location_Id",
                principalTable: "Business_Location",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
