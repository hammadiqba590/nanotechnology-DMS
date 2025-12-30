using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoDMSAdminService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                .Annotation("Npgsql:Enum:psp_payment_type_status", "unknown,card,wallet,bank_transfer,upi,qr")
                .Annotation("Npgsql:Enum:record_status", "unknown,active,inactive")
                .Annotation("Npgsql:Enum:settlement_frequency_status", "unknown,daily,weekly,monthly")
                .Annotation("Npgsql:Enum:tax_on_merchant_status", "unknown,yes,no")
                .Annotation("Npgsql:Enum:terminal_history_status", "unknown,active,inactive,maintenance,decommissioned");

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Campaign_Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Tax_Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    Fbr = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true, defaultValue: 2),
                    Budget_Limit_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 5),
                    Budget_Limit_Value = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Card_Brands",
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
                    table.PrimaryKey("PK_Card_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Card_Levels",
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
                    table.PrimaryKey("PK_Card_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Card_Types",
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
                    table.PrimaryKey("PK_Card_Types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Iso2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Iso3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Numeric_Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Phone_Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Currency_Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Currency_Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Flag_Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Time_Zone = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pos_Terminal_Masters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Serial_Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Terminal_Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Company = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Model_Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Software_Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Pos_Terminal_Masters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Psp_Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
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
                    table.PrimaryKey("PK_Psp_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Short_Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Short_Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Swift_Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country_Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_Countries_Country_Id",
                        column: x => x.Country_Id,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Country_Id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_Countries_Country_Id",
                        column: x => x.Country_Id,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pos_Terminal_Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PosTerminal_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Assigned_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Unassigned_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_Pos_Terminal_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pos_Terminal_Assignments_Pos_Terminal_Masters_PosTerminal_Id",
                        column: x => x.PosTerminal_Id,
                        principalTable: "Pos_Terminal_Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pos_Terminal_Configurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Terminal_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Config_Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Config_Value = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_Pos_Terminal_Configurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pos_Terminal_Configurations_Pos_Terminal_Masters_Terminal_Id",
                        column: x => x.Terminal_Id,
                        principalTable: "Pos_Terminal_Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pos_Terminal_Status_Histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Terminal_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Notes = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Pos_Terminal_Status_Histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pos_Terminal_Status_Histories_Pos_Terminal_Masters_Terminal~",
                        column: x => x.Terminal_Id,
                        principalTable: "Pos_Terminal_Masters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Psps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Short_Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Psp_Category_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Country_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Currency_Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Currency_Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Registration_Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Reg_Doc_Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Compliance_Status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Contact_Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Contact_Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Api_Endpoint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Sandbox_Endpoint = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Webhook_Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Api_Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Documentation_Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Integration_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Supported_Payment_Methods = table.Column<string>(type: "text", nullable: true),
                    Supported_Currencies = table.Column<string>(type: "text", nullable: true),
                    Settlement_Frequency = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Transaction_Limit = table.Column<decimal>(type: "numeric", nullable: true),
                    Daily_Volume_Limit = table.Column<decimal>(type: "numeric", nullable: true),
                    Risk_Score = table.Column<int>(type: "integer", nullable: true),
                    Requires_Kyc = table.Column<bool>(type: "boolean", nullable: false),
                    Onboarded_By = table.Column<Guid>(type: "uuid", nullable: true),
                    Onboarded_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_Psps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psps_Countries_Country_Id",
                        column: x => x.Country_Id,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Psps_Psp_Categories_Psp_Category_Id",
                        column: x => x.Psp_Category_Id,
                        principalTable: "Psp_Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Campaign_Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Campagin_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bank_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: false),
                    Discount_Share = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax_On_Merchant_Share = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Budget_Limit_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 5),
                    Budget_Limit_Value = table.Column<int>(type: "integer", nullable: true),
                    Discount_Mode = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
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
                    table.PrimaryKey("PK_Campaign_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaign_Banks_Banks_Bank_Id",
                        column: x => x.Bank_Id,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campaign_Banks_Campaigns_Campagin_Id",
                        column: x => x.Campagin_Id,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Card_Bins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bank_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Card_Bin_Value = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Card_Brand_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Card_Type_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Card_Level_Id = table.Column<Guid>(type: "uuid", nullable: true),
                    Local_International = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Country_Id = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Card_Bins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_Bins_Banks_Bank_Id",
                        column: x => x.Bank_Id,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_Bins_Card_Brands_Card_Brand_Id",
                        column: x => x.Card_Brand_Id,
                        principalTable: "Card_Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_Bins_Card_Levels_Card_Level_Id",
                        column: x => x.Card_Level_Id,
                        principalTable: "Card_Levels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Card_Bins_Card_Types_Card_Type_Id",
                        column: x => x.Card_Type_Id,
                        principalTable: "Card_Types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Card_Bins_Countries_Country_Id",
                        column: x => x.Country_Id,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Psp_Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Psp_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency_Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Psp_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psp_Currencies_Currencies_Currency_Id",
                        column: x => x.Currency_Id,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Psp_Currencies_Psps_Psp_Id",
                        column: x => x.Psp_Id,
                        principalTable: "Psps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Psp_Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Psp_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Doc_Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Doc_Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
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
                    table.PrimaryKey("PK_Psp_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psp_Documents_Psps_Psp_Id",
                        column: x => x.Psp_Id,
                        principalTable: "Psps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Psp_Payment_Methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Psp_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Payment_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
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
                    table.PrimaryKey("PK_Psp_Payment_Methods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Psp_Payment_Methods_Psps_Psp_Id",
                        column: x => x.Psp_Id,
                        principalTable: "Psps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Campaign_Card_Bins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Campagin_Bank_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Card_Bin_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
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
                    table.PrimaryKey("PK_Campaign_Card_Bins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaign_Card_Bins_Campaign_Banks_Campagin_Bank_Id",
                        column: x => x.Campagin_Bank_Id,
                        principalTable: "Campaign_Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Campaign_Card_Bins_Card_Bins_Card_Bin_Id",
                        column: x => x.Card_Bin_Id,
                        principalTable: "Card_Bins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discount_Rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Campaign_Card_Bin_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discount_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Discount_Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Min_Spend = table.Column<decimal>(type: "numeric", nullable: true),
                    Max_Discount = table.Column<decimal>(type: "numeric", nullable: true),
                    Payment_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Budget_Limit_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 5),
                    Budget_Limit_Value = table.Column<int>(type: "integer", nullable: true),
                    Applicable_Days = table.Column<string>(type: "text", nullable: true),
                    Transaction_Cap = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Start_Time = table.Column<TimeSpan>(type: "interval", nullable: true),
                    End_Time = table.Column<TimeSpan>(type: "interval", nullable: true),
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
                    table.PrimaryKey("PK_Discount_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_Rules_Campaign_Card_Bins_Campaign_Card_Bin_Id",
                        column: x => x.Campaign_Card_Bin_Id,
                        principalTable: "Campaign_Card_Bins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discount_Rule_Histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discount_Rule_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Campaign_Card_Bin_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discount_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Discount_Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency_Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Min_Spend = table.Column<decimal>(type: "numeric", nullable: true),
                    Max_Discount = table.Column<decimal>(type: "numeric", nullable: true),
                    Payment_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
                    Applicable_Days = table.Column<string>(type: "text", nullable: true),
                    Transaction_Cap = table.Column<int>(type: "integer", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Stackable = table.Column<bool>(type: "boolean", nullable: false),
                    Change_Type = table.Column<int>(type: "integer", nullable: true, defaultValue: 1),
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
                    table.PrimaryKey("PK_Discount_Rule_Histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_Rule_Histories_Campaign_Card_Bins_Campaign_Card_Bi~",
                        column: x => x.Campaign_Card_Bin_Id,
                        principalTable: "Campaign_Card_Bins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Discount_Rule_Histories_Currencies_Currency_Id",
                        column: x => x.Currency_Id,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Discount_Rule_Histories_Discount_Rules_Discount_Rule_Id",
                        column: x => x.Discount_Rule_Id,
                        principalTable: "Discount_Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_Country_Id",
                table: "Banks",
                column: "Country_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_Name",
                table: "Banks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banks_Short_Code",
                table: "Banks",
                column: "Short_Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_Banks_Bank_Id",
                table: "Campaign_Banks",
                column: "Bank_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_Banks_Campagin_Id",
                table: "Campaign_Banks",
                column: "Campagin_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_Card_Bins_Campagin_Bank_Id",
                table: "Campaign_Card_Bins",
                column: "Campagin_Bank_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaign_Card_Bins_Card_Bin_Id",
                table: "Campaign_Card_Bins",
                column: "Card_Bin_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_Campaign_Name",
                table: "Campaigns",
                column: "Campaign_Name");

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Bank_Id",
                table: "Card_Bins",
                column: "Bank_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Card_Bin_Value",
                table: "Card_Bins",
                column: "Card_Bin_Value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Card_Brand_Id",
                table: "Card_Bins",
                column: "Card_Brand_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Card_Level_Id",
                table: "Card_Bins",
                column: "Card_Level_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Card_Type_Id",
                table: "Card_Bins",
                column: "Card_Type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Card_Bins_Country_Id",
                table: "Card_Bins",
                column: "Country_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso2",
                table: "Countries",
                column: "Iso2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Iso3",
                table: "Countries",
                column: "Iso3",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Country_Id",
                table: "Currencies",
                column: "Country_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Rule_Histories_Campaign_Card_Bin_Id",
                table: "Discount_Rule_Histories",
                column: "Campaign_Card_Bin_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Rule_Histories_Currency_Id",
                table: "Discount_Rule_Histories",
                column: "Currency_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Rule_Histories_Discount_Rule_Id",
                table: "Discount_Rule_Histories",
                column: "Discount_Rule_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_Rules_Campaign_Card_Bin_Id",
                table: "Discount_Rules",
                column: "Campaign_Card_Bin_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Assignments_PosTerminal_Id",
                table: "Pos_Terminal_Assignments",
                column: "PosTerminal_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Assignments_Tid",
                table: "Pos_Terminal_Assignments",
                column: "Tid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Configurations_Config_Key",
                table: "Pos_Terminal_Configurations",
                column: "Config_Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Configurations_Terminal_Id",
                table: "Pos_Terminal_Configurations",
                column: "Terminal_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Masters_Serial_Number",
                table: "Pos_Terminal_Masters",
                column: "Serial_Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pos_Terminal_Status_Histories_Terminal_Id",
                table: "Pos_Terminal_Status_Histories",
                column: "Terminal_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Psp_Categories_Name",
                table: "Psp_Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Psp_Currencies_Currency_Id",
                table: "Psp_Currencies",
                column: "Currency_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Psp_Currencies_Psp_Id_Currency_Id",
                table: "Psp_Currencies",
                columns: new[] { "Psp_Id", "Currency_Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Psp_Documents_Psp_Id",
                table: "Psp_Documents",
                column: "Psp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Psp_Payment_Methods_Psp_Id",
                table: "Psp_Payment_Methods",
                column: "Psp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Psps_Code",
                table: "Psps",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Psps_Country_Id",
                table: "Psps",
                column: "Country_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Psps_Name",
                table: "Psps",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Psps_Psp_Category_Id",
                table: "Psps",
                column: "Psp_Category_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discount_Rule_Histories");

            migrationBuilder.DropTable(
                name: "Pos_Terminal_Assignments");

            migrationBuilder.DropTable(
                name: "Pos_Terminal_Configurations");

            migrationBuilder.DropTable(
                name: "Pos_Terminal_Status_Histories");

            migrationBuilder.DropTable(
                name: "Psp_Currencies");

            migrationBuilder.DropTable(
                name: "Psp_Documents");

            migrationBuilder.DropTable(
                name: "Psp_Payment_Methods");

            migrationBuilder.DropTable(
                name: "Discount_Rules");

            migrationBuilder.DropTable(
                name: "Pos_Terminal_Masters");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Psps");

            migrationBuilder.DropTable(
                name: "Campaign_Card_Bins");

            migrationBuilder.DropTable(
                name: "Psp_Categories");

            migrationBuilder.DropTable(
                name: "Campaign_Banks");

            migrationBuilder.DropTable(
                name: "Card_Bins");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Card_Brands");

            migrationBuilder.DropTable(
                name: "Card_Levels");

            migrationBuilder.DropTable(
                name: "Card_Types");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
