using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imperium_Incursions_Waitlist.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    JabberNotifications = table.Column<bool>(nullable: false, defaultValue: true),
                    LastLoginIP = table.Column<string>(maxLength: 15, nullable: true),
                    RegisteredAt = table.Column<DateTime>(nullable: false),
                    LastLogin = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alliance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommChannels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: false),
                    LinkText = table.Column<string>(nullable: false)
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Acronym = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Avaliable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Slot = table.Column<string>(nullable: true),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Systems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorAdminId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false, defaultValue: "primary"),
                    Message = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Announcements_Accounts_CreatorAdminId",
                        column: x => x.CreatorAdminId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<int>(nullable: false),
                    UpdatedByAdminId = table.Column<int>(nullable: true),
                    BannedAccountId = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bans_Accounts_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bans_Accounts_BannedAccountId",
                        column: x => x.BannedAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bans_Accounts_UpdatedByAdminId",
                        column: x => x.UpdatedByAdminId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<int>(nullable: false),
                    TargetAccountId = table.Column<int>(nullable: false),
                    UpdatedByAdminId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: false),
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
                name: "Corporation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AllianceId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Corporation_Alliance_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountRoles",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRoles", x => new { x.AccountId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AccountRoles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    ShipTypeId = table.Column<int>(nullable: false),
                    FittingDNA = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
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
                name: "Pilots",
                columns: table => new
                {
                    CharacterID = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    CharacterName = table.Column<string>(nullable: false),
                    CorporationID = table.Column<long>(nullable: false),
                    RefreshToken = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true),
                    RegisteredAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilots", x => x.CharacterID);
                    table.ForeignKey(
                        name: "FK_Pilots_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pilots_Corporation_CorporationID",
                        column: x => x.CorporationID,
                        principalTable: "Corporation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fleets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EveFleetId = table.Column<long>(nullable: false),
                    BossPilotId = table.Column<int>(nullable: true),
                    CommChannelId = table.Column<int>(nullable: false),
                    ErrorCount = table.Column<int>(nullable: true),
                    SystemId = table.Column<int>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Wings = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ClosedAt = table.Column<DateTime>(nullable: true),
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
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fleets_CommChannels_CommChannelId",
                        column: x => x.CommChannelId,
                        principalTable: "CommChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fleets_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PilotSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaitingPilots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PilotId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: true),
                    RemovedByAccountId = table.Column<int>(nullable: true),
                    NewPilot = table.Column<bool>(nullable: false),
                    OfflineAt = table.Column<DateTime>(nullable: true),
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
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WaitingPilots_Accounts_RemovedByAccountId",
                        column: x => x.RemovedByAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitingPilots_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FleetAssignments",
                columns: table => new
                {
                    WaitingPilotId = table.Column<int>(nullable: false),
                    FleetId = table.Column<int>(nullable: false),
                    SystemId = table.Column<int>(nullable: false),
                    ShipTypeId = table.Column<int>(nullable: true),
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
                        name: "FK_FleetAssignments_ShipTypes_ShipTypeId",
                        column: x => x.ShipTypeId,
                        principalTable: "ShipTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FleetAssignments_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
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

            migrationBuilder.InsertData(
                table: "Alliance",
                columns: new[] { "Id", "Name" },
                values: new object[] { 0, "" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRoles_RoleId",
                table: "AccountRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_CreatorAdminId",
                table: "Announcements",
                column: "CreatorAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_AdminId",
                table: "Bans",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_BannedAccountId",
                table: "Bans",
                column: "BannedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Bans_UpdatedByAdminId",
                table: "Bans",
                column: "UpdatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Corporation_AllianceId",
                table: "Corporation",
                column: "AllianceId");

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
                name: "IX_FleetAssignments_ShipTypeId",
                table: "FleetAssignments",
                column: "ShipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FleetAssignments_SystemId",
                table: "FleetAssignments",
                column: "SystemId");

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
                name: "IX_Fleets_SystemId",
                table: "Fleets",
                column: "SystemId");

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
                name: "IX_Pilots_AccountId",
                table: "Pilots",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Pilots_CorporationID",
                table: "Pilots",
                column: "CorporationID");

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

            migrationBuilder.CreateIndex(
                name: "IX_WaitingPilots_SystemId",
                table: "WaitingPilots",
                column: "SystemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRoles");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Bans");

            migrationBuilder.DropTable(
                name: "FleetAssignments");

            migrationBuilder.DropTable(
                name: "Modules");

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
                name: "Roles");

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

            migrationBuilder.DropTable(
                name: "Pilots");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Corporation");

            migrationBuilder.DropTable(
                name: "Alliance");
        }
    }
}
