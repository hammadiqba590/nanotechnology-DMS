using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSSetupService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Financial_Year_Start_Month",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financial_Year_Start_Month", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marital_Status",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marital_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Country_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stock_Accounting_Method",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock_Accounting_Method", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Time_Zone",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    GMT_Setting = table.Column<string>(type: "text", nullable: false),
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
                    End_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Time_Zone", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Financial_Year_Start_Month");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "Marital_Status");

            migrationBuilder.DropTable(
                name: "State");

            migrationBuilder.DropTable(
                name: "Stock_Accounting_Method");

            migrationBuilder.DropTable(
                name: "Time_Zone");
        }
    }
}
