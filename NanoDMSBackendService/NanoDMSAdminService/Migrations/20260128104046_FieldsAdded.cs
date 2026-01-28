using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class FieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:budget_limit_type_status", "unknown,hourly,daily,weekly,monthly,yearly")
                .Annotation("Npgsql:Enum:campagin_card_bin_status", "unknown,active,inactive")
                .Annotation("Npgsql:Enum:campagin_status", "unkown,active,inactive,expired")
                .Annotation("Npgsql:Enum:change_type_status", "unknown,insert,update,delete")
                .Annotation("Npgsql:Enum:compliance_status", "unknown,pending,approved,rejected")
                .Annotation("Npgsql:Enum:discount_mode_status", "unknown,all,swipe,dip,tap")
                .Annotation("Npgsql:Enum:discount_type_status", "unknown,percentage,flat")
                .Annotation("Npgsql:Enum:integration_type_status", "unknown,direct,aggregator,gateway")
                .Annotation("Npgsql:Enum:local_international_status", "unknown,local,international")
                .Annotation("Npgsql:Enum:payment_type_status", "unknown,all,card,wallet,qr")
                .Annotation("Npgsql:Enum:pos_mode_status", "unknown,all,dinein,takeaway,delivery")
                .Annotation("Npgsql:Enum:psp_payment_type_status", "unknown,card,wallet,bank_transfer,upi,qr")
                .Annotation("Npgsql:Enum:record_status", "unknown,active,inactive")
                .Annotation("Npgsql:Enum:settlement_frequency_status", "unknown,daily,weekly,monthly")
                .Annotation("Npgsql:Enum:tax_on_merchant_status", "unknown,yes,no")
                .Annotation("Npgsql:Enum:terminal_history_status", "unknown,active,inactive,maintenance,decommissioned")
                .OldAnnotation("Npgsql:Enum:budget_limit_type_status", "unknown,hourly,daily,weekly,monthly,yearly")
                .OldAnnotation("Npgsql:Enum:campagin_card_bin_status", "unknown,active,inactive")
                .OldAnnotation("Npgsql:Enum:campagin_status", "unkown,active,inactive,expired")
                .OldAnnotation("Npgsql:Enum:change_type_status", "unknown,insert,update,delete")
                .OldAnnotation("Npgsql:Enum:compliance_status", "unknown,pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:discount_mode_status", "unknown,all,swipe,dip,tap")
                .OldAnnotation("Npgsql:Enum:discount_type_status", "unknown,percentage,flat")
                .OldAnnotation("Npgsql:Enum:integration_type_status", "unknown,direct,aggregator,gateway")
                .OldAnnotation("Npgsql:Enum:local_international_status", "unknown,local,international")
                .OldAnnotation("Npgsql:Enum:payment_type_status", "unknown,all,card,wallet,qr")
                .OldAnnotation("Npgsql:Enum:psp_payment_type_status", "unknown,card,wallet,bank_transfer,upi,qr")
                .OldAnnotation("Npgsql:Enum:record_status", "unknown,active,inactive")
                .OldAnnotation("Npgsql:Enum:settlement_frequency_status", "unknown,daily,weekly,monthly")
                .OldAnnotation("Npgsql:Enum:tax_on_merchant_status", "unknown,yes,no")
                .OldAnnotation("Npgsql:Enum:terminal_history_status", "unknown,active,inactive,maintenance,decommissioned");

            migrationBuilder.AddColumn<int>(
                name: "Discount_Mode",
                table: "Discount_Rules",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Pos_Mode",
                table: "Discount_Rules",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Discount_Mode",
                table: "Discount_Rule_Histories",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Pos_Mode",
                table: "Discount_Rule_Histories",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "Bank_Share",
                table: "Campaign_Banks",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount_Mode",
                table: "Discount_Rules");

            migrationBuilder.DropColumn(
                name: "Pos_Mode",
                table: "Discount_Rules");

            migrationBuilder.DropColumn(
                name: "Discount_Mode",
                table: "Discount_Rule_Histories");

            migrationBuilder.DropColumn(
                name: "Pos_Mode",
                table: "Discount_Rule_Histories");

            migrationBuilder.DropColumn(
                name: "Bank_Share",
                table: "Campaign_Banks");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:budget_limit_type_status", "unknown,hourly,daily,weekly,monthly,yearly")
                .Annotation("Npgsql:Enum:campagin_card_bin_status", "unknown,active,inactive")
                .Annotation("Npgsql:Enum:campagin_status", "unkown,active,inactive,expired")
                .Annotation("Npgsql:Enum:change_type_status", "unknown,insert,update,delete")
                .Annotation("Npgsql:Enum:compliance_status", "unknown,pending,approved,rejected")
                .Annotation("Npgsql:Enum:discount_mode_status", "unknown,all,swipe,dip,tap")
                .Annotation("Npgsql:Enum:discount_type_status", "unknown,percentage,flat")
                .Annotation("Npgsql:Enum:integration_type_status", "unknown,direct,aggregator,gateway")
                .Annotation("Npgsql:Enum:local_international_status", "unknown,local,international")
                .Annotation("Npgsql:Enum:payment_type_status", "unknown,all,card,wallet,qr")
                .Annotation("Npgsql:Enum:psp_payment_type_status", "unknown,card,wallet,bank_transfer,upi,qr")
                .Annotation("Npgsql:Enum:record_status", "unknown,active,inactive")
                .Annotation("Npgsql:Enum:settlement_frequency_status", "unknown,daily,weekly,monthly")
                .Annotation("Npgsql:Enum:tax_on_merchant_status", "unknown,yes,no")
                .Annotation("Npgsql:Enum:terminal_history_status", "unknown,active,inactive,maintenance,decommissioned")
                .OldAnnotation("Npgsql:Enum:budget_limit_type_status", "unknown,hourly,daily,weekly,monthly,yearly")
                .OldAnnotation("Npgsql:Enum:campagin_card_bin_status", "unknown,active,inactive")
                .OldAnnotation("Npgsql:Enum:campagin_status", "unkown,active,inactive,expired")
                .OldAnnotation("Npgsql:Enum:change_type_status", "unknown,insert,update,delete")
                .OldAnnotation("Npgsql:Enum:compliance_status", "unknown,pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:discount_mode_status", "unknown,all,swipe,dip,tap")
                .OldAnnotation("Npgsql:Enum:discount_type_status", "unknown,percentage,flat")
                .OldAnnotation("Npgsql:Enum:integration_type_status", "unknown,direct,aggregator,gateway")
                .OldAnnotation("Npgsql:Enum:local_international_status", "unknown,local,international")
                .OldAnnotation("Npgsql:Enum:payment_type_status", "unknown,all,card,wallet,qr")
                .OldAnnotation("Npgsql:Enum:pos_mode_status", "unknown,all,dinein,takeaway,delivery")
                .OldAnnotation("Npgsql:Enum:psp_payment_type_status", "unknown,card,wallet,bank_transfer,upi,qr")
                .OldAnnotation("Npgsql:Enum:record_status", "unknown,active,inactive")
                .OldAnnotation("Npgsql:Enum:settlement_frequency_status", "unknown,daily,weekly,monthly")
                .OldAnnotation("Npgsql:Enum:tax_on_merchant_status", "unknown,yes,no")
                .OldAnnotation("Npgsql:Enum:terminal_history_status", "unknown,active,inactive,maintenance,decommissioned");
        }
    }
}
