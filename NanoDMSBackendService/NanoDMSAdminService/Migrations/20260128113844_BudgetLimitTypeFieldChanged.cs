using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class BudgetLimitTypeFieldChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget_Limit_Value",
                table: "Discount_Rules");

            migrationBuilder.AlterColumn<string>(
                name: "Budget_Limit_Type",
                table: "Discount_Rules",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Budget_Limit_Type",
                table: "Discount_Rules",
                type: "integer",
                nullable: true,
                defaultValue: 5,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Budget_Limit_Value",
                table: "Discount_Rules",
                type: "integer",
                nullable: true);
        }
    }
}
