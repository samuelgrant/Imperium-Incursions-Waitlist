using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class AddModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommChannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: true),
                    LinkText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FleetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<int>(nullable: false),
                    TargetAccountId = table.Column<int>(nullable: false),
                    UpdatedByAdminId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Accounts_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notes_Accounts_TargetAccountId",
                        column: x => x.TargetAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Accounts_UpdatedByAdminId",
                        column: x => x.UpdatedByAdminId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Queue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaitingPilots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PilotId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false),
                    RemovedByAccountId = table.Column<int>(nullable: true),
                    NewPilot = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitingPilots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaitingPilots_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaitingPilots_Accounts_RemovedByAccountId",
                        column: x => x.RemovedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fleets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EveFleetId = table.Column<int>(nullable: false),
                    BossId = table.Column<int>(nullable: false),
                    BackseatId = table.Column<int>(nullable: false),
                    CommChannelId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    BossPilotId = table.Column<int>(nullable: true),
                    BackseatAccountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fleets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fleets_Accounts_BackseatAccountId",
                        column: x => x.BackseatAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fleets_Pilots_BossPilotId",
                        column: x => x.BossPilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fleets_CommChannels_CommChannelId",
                        column: x => x.CommChannelId,
                        principalTable: "CommChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    ShipTypeId = table.Column<int>(nullable: false),
                    FittingDNA = table.Column<string>(nullable: true),
                    IsShipScan = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fits_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fits_ShipTypes_ShipTypeId",
                        column: x => x.ShipTypeId,
                        principalTable: "ShipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PilotSkills",
                columns: table => new
                {
                    PilotId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PilotSkills", x => new { x.PilotId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_PilotSkills_Pilots_PilotId",
                        column: x => x.PilotId,
                        principalTable: "Pilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PilotSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipSkills",
                columns: table => new
                {
                    ShipTypeId = table.Column<int>(nullable: false),
                    SkillId = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipSkills", x => new { x.ShipTypeId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_ShipSkills_ShipTypes_ShipTypeId",
                        column: x => x.ShipTypeId,
                        principalTable: "ShipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedRoles",
                columns: table => new
                {
                    WaitingPilotId = table.Column<int>(nullable: false),
                    FleetRoleId = table.Column<int>(nullable: false),
                    ToFleet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedRoles", x => new { x.WaitingPilotId, x.FleetRoleId });
                    table.ForeignKey(
                        name: "FK_SelectedRoles_FleetRoles_FleetRoleId",
                        column: x => x.FleetRoleId,
                        principalTable: "FleetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedRoles_WaitingPilots_WaitingPilotId",
                        column: x => x.WaitingPilotId,
                        principalTable: "WaitingPilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FleetAssignments",
                columns: table => new
                {
                    WaitingPilotId = table.Column<int>(nullable: false),
                    FleetId = table.Column<int>(nullable: false),
                    IsExitCyno = table.Column<bool>(nullable: false),
                    TakesFleetWarp = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    DeletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetAssignments", x => new { x.WaitingPilotId, x.FleetId });
                    table.ForeignKey(
                        name: "FK_FleetAssignments_Fleets_FleetId",
                        column: x => x.FleetId,
                        principalTable: "Fleets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FleetAssignments_WaitingPilots_WaitingPilotId",
                        column: x => x.WaitingPilotId,
                        principalTable: "WaitingPilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectedFits",
                columns: table => new
                {
                    WaitingPilotId = table.Column<int>(nullable: false),
                    FitId = table.Column<int>(nullable: false),
                    ToFleet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedFits", x => new { x.WaitingPilotId, x.FitId });
                    table.ForeignKey(
                        name: "FK_SelectedFits_Fits_FitId",
                        column: x => x.FitId,
                        principalTable: "Fits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SelectedFits_WaitingPilots_WaitingPilotId",
                        column: x => x.WaitingPilotId,
                        principalTable: "WaitingPilots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fits_AccountId",
                table: "Fits",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Fits_ShipTypeId",
                table: "Fits",
                column: "ShipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FleetAssignments_FleetId",
                table: "FleetAssignments",
                column: "FleetId");

            migrationBuilder.CreateIndex(
                name: "IX_FleetAssignments_WaitingPilotId",
                table: "FleetAssignments",
                column: "WaitingPilotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_BackseatAccountId",
                table: "Fleets",
                column: "BackseatAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_BossPilotId",
                table: "Fleets",
                column: "BossPilotId");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_CommChannelId",
                table: "Fleets",
                column: "CommChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_AdminId",
                table: "Notes",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_TargetAccountId",
                table: "Notes",
                column: "TargetAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UpdatedByAdminId",
                table: "Notes",
                column: "UpdatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PilotSkills_SkillId",
                table: "PilotSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedFits_FitId",
                table: "SelectedFits",
                column: "FitId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectedRoles_FleetRoleId",
                table: "SelectedRoles",
                column: "FleetRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipSkills_SkillId",
                table: "ShipSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPilots_PilotId",
                table: "WaitingPilots",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPilots_RemovedByAccountId",
                table: "WaitingPilots",
                column: "RemovedByAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FleetAssignments");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "PilotSkills");

            migrationBuilder.DropTable(
                name: "SelectedFits");

            migrationBuilder.DropTable(
                name: "SelectedRoles");

            migrationBuilder.DropTable(
                name: "ShipSkills");

            migrationBuilder.DropTable(
                name: "Fleets");

            migrationBuilder.DropTable(
                name: "Fits");

            migrationBuilder.DropTable(
                name: "FleetRoles");

            migrationBuilder.DropTable(
                name: "WaitingPilots");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "CommChannels");

            migrationBuilder.DropTable(
                name: "ShipTypes");
        }
    }
}
