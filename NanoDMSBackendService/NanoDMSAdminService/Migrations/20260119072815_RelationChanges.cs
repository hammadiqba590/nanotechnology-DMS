using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class RelationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Psp_Id",
                table: "Campaigns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Campagin_Id",
                table: "Campaign_Card_Bins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Psp_Id",
                table: "Campaigns",
                column: "Psp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_Card_Bins_Campagin_Id",
                table: "Campaign_Card_Bins",
                column: "Campagin_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaign_Card_Bins_Campaigns_Campagin_Id",
                table: "Campaign_Card_Bins",
                column: "Campagin_Id",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_Psps_Psp_Id",
                table: "Campaigns",
                column: "Psp_Id",
                principalTable: "Psps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaign_Card_Bins_Campaigns_Campagin_Id",
                table: "Campaign_Card_Bins");

            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_Psps_Psp_Id",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaigns_Psp_Id",
                table: "Campaigns");

            migrationBuilder.DropIndex(
                name: "IX_Campaign_Card_Bins_Campagin_Id",
                table: "Campaign_Card_Bins");

            migrationBuilder.DropColumn(
                name: "Psp_Id",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Campagin_Id",
                table: "Campaign_Card_Bins");
        }
    }
}
