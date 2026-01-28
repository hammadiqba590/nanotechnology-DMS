using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSBusinessService.Migrations
{
    /// <inheritdoc />
    public partial class LocationFieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiscountBeforePosCharge",
                table: "Business_Location",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountBeforeServiceCharge",
                table: "Business_Location",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiscountBeforeTax",
                table: "Business_Location",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PosCharge",
                table: "Business_Location",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceCharges",
                table: "Business_Location",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountBeforePosCharge",
                table: "Business_Location");

            migrationBuilder.DropColumn(
                name: "DiscountBeforeServiceCharge",
                table: "Business_Location");

            migrationBuilder.DropColumn(
                name: "DiscountBeforeTax",
                table: "Business_Location");

            migrationBuilder.DropColumn(
                name: "PosCharge",
                table: "Business_Location");

            migrationBuilder.DropColumn(
                name: "ServiceCharges",
                table: "Business_Location");
        }
    }
}
