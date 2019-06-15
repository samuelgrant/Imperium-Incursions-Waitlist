﻿// <auto-generated />
using System;
using Imperium_Incursions_Waitlist.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Imperium_Incursions_Waitlist.Migrations
{
    [DbContext(typeof(WaitlistDataContext))]
    partial class WaitlistDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Account", b =>
                {
                    b.Property<int>("Id");

                    b.Property<bool>("JabberNotifications")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("LastLogin");

                    b.Property<string>("LastLoginIP")
                        .HasMaxLength(15);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<DateTime>("RegisteredAt");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.AccountRole", b =>
                {
                    b.Property<int>("AccountId");

                    b.Property<int>("RoleId");

                    b.HasKey("AccountId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AccountRoles");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Alliance", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Alliance");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = ""
                        });
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Announcement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("CreatorAdminId");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue("primary");

                    b.HasKey("Id");

                    b.HasIndex("CreatorAdminId");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Ban", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AdminId");

                    b.Property<int>("BannedAccountId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("ExpiresAt");

                    b.Property<string>("Reason");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int?>("UpdatedByAdminId");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("BannedAccountId");

                    b.HasIndex("UpdatedByAdminId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.CommChannel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LinkText")
                        .IsRequired();

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("CommChannels");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Corporation", b =>
                {
                    b.Property<long>("Id");

                    b.Property<int?>("AllianceId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId");

                    b.ToTable("Corporation");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Fit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<string>("Description");

                    b.Property<string>("FittingDNA");

                    b.Property<bool>("IsShipScan");

                    b.Property<int>("ShipTypeId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ShipTypeId");

                    b.ToTable("Fits");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Fleet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BackseatAccountId");

                    b.Property<int?>("BossPilotId");

                    b.Property<DateTime?>("ClosedAt");

                    b.Property<int>("CommChannelId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("ErrorCount");

                    b.Property<long>("EveFleetId");

                    b.Property<bool>("IsPublic");

                    b.Property<int?>("SystemId");

                    b.Property<string>("Type");

                    b.Property<DateTime?>("UpdatedAt");

                    b.Property<string>("Wings");

                    b.HasKey("Id");

                    b.HasIndex("BackseatAccountId");

                    b.HasIndex("BossPilotId");

                    b.HasIndex("CommChannelId");

                    b.HasIndex("SystemId");

                    b.ToTable("Fleets");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.FleetAssignment", b =>
                {
                    b.Property<int?>("WaitingPilotId");

                    b.Property<int>("FleetId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<DateTime?>("DeletedAt");

                    b.Property<bool>("IsExitCyno");

                    b.Property<int?>("ShipTypeId");

                    b.Property<int>("SystemId");

                    b.Property<bool>("TakesFleetWarp");

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("WaitingPilotId", "FleetId");

                    b.HasIndex("FleetId");

                    b.HasIndex("ShipTypeId");

                    b.HasIndex("SystemId");

                    b.HasIndex("WaitingPilotId")
                        .IsUnique();

                    b.ToTable("FleetAssignments");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.FleetRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Acronym")
                        .IsRequired();

                    b.Property<bool>("Avaliable");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("FleetRoles");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.ModuleItem", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("GroupId");

                    b.Property<string>("Name");

                    b.Property<string>("Slot");

                    b.HasKey("Id");

                    b.ToTable("Modules");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AdminId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<int>("TargetAccountId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UpdatedByAdminId");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("TargetAccountId");

                    b.HasIndex("UpdatedByAdminId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Pilot", b =>
                {
                    b.Property<int>("CharacterID");

                    b.Property<int>("AccountId");

                    b.Property<string>("CharacterName")
                        .IsRequired();

                    b.Property<long>("CorporationID");

                    b.Property<string>("RefreshToken");

                    b.Property<DateTime>("RegisteredAt");

                    b.Property<string>("Token");

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("CharacterID");

                    b.HasIndex("AccountId");

                    b.HasIndex("CorporationID");

                    b.ToTable("Pilots");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.PilotSkill", b =>
                {
                    b.Property<int>("PilotId");

                    b.Property<int>("SkillId");

                    b.Property<int>("Level");

                    b.HasKey("PilotId", "SkillId");

                    b.HasIndex("SkillId");

                    b.ToTable("PilotSkills");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.SelectedFit", b =>
                {
                    b.Property<int>("WaitingPilotId");

                    b.Property<int>("FitId");

                    b.Property<bool>("ToFleet");

                    b.HasKey("WaitingPilotId", "FitId");

                    b.HasIndex("FitId");

                    b.ToTable("SelectedFits");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.SelectedRole", b =>
                {
                    b.Property<int>("WaitingPilotId");

                    b.Property<int>("FleetRoleId");

                    b.Property<bool>("ToFleet");

                    b.HasKey("WaitingPilotId", "FleetRoleId");

                    b.HasIndex("FleetRoleId");

                    b.ToTable("SelectedRoles");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.ShipSkill", b =>
                {
                    b.Property<int>("ShipTypeId");

                    b.Property<int>("SkillId");

                    b.Property<bool>("IsRequired");

                    b.Property<int>("Level");

                    b.HasKey("ShipTypeId", "SkillId");

                    b.HasIndex("SkillId");

                    b.ToTable("ShipSkills");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.ShipType", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Queue");

                    b.HasKey("Id");

                    b.ToTable("ShipTypes");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.StarSystem", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Systems");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.WaitingPilot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("NewPilot");

                    b.Property<DateTime?>("OfflineAt");

                    b.Property<int>("PilotId");

                    b.Property<int?>("RemovedByAccountId");

                    b.Property<int?>("SystemId");

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("PilotId");

                    b.HasIndex("RemovedByAccountId");

                    b.HasIndex("SystemId");

                    b.ToTable("WaitingPilots");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.AccountRole", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "Account")
                        .WithMany("AccountRoles")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Role", "Role")
                        .WithMany("AccountRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Announcement", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "CreatorAdmin")
                        .WithMany()
                        .HasForeignKey("CreatorAdminId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Ban", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "CreatorAdmin")
                        .WithMany("CreatedBans")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "BannedAccount")
                        .WithMany("AccountBans")
                        .HasForeignKey("BannedAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "UpdatingAdmin")
                        .WithMany("UpdatedBans")
                        .HasForeignKey("UpdatedByAdminId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Corporation", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Alliance", "Alliance")
                        .WithMany("Corporations")
                        .HasForeignKey("AllianceId");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Fit", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account")
                        .WithMany("Fits")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.ShipType", "ShipType")
                        .WithMany("Fits")
                        .HasForeignKey("ShipTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Fleet", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "BackseatAccount")
                        .WithMany("BackseatedFleets")
                        .HasForeignKey("BackseatAccountId");

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Pilot", "BossPilot")
                        .WithMany("OwnedFleets")
                        .HasForeignKey("BossPilotId");

                    b.HasOne("Imperium_Incursions_Waitlist.Models.CommChannel", "CommChannel")
                        .WithMany("Fleets")
                        .HasForeignKey("CommChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.StarSystem", "System")
                        .WithMany()
                        .HasForeignKey("SystemId");
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.FleetAssignment", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Fleet", "Fleet")
                        .WithMany("FleetAssignments")
                        .HasForeignKey("FleetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.ShipType", "ActiveShip")
                        .WithMany()
                        .HasForeignKey("ShipTypeId");

                    b.HasOne("Imperium_Incursions_Waitlist.Models.StarSystem", "System")
                        .WithMany()
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.WaitingPilot", "WaitingPilot")
                        .WithOne("FleetAssignment")
                        .HasForeignKey("Imperium_Incursions_Waitlist.Models.FleetAssignment", "WaitingPilotId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Note", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "CreatorAdmin")
                        .WithMany("CreatedNotes")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "TargetAccount")
                        .WithMany("AccountNotes")
                        .HasForeignKey("TargetAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "UpdatingAdmin")
                        .WithMany("UpdatedNotes")
                        .HasForeignKey("UpdatedByAdminId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.Pilot", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "Account")
                        .WithMany("Pilots")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Corporation", "Corporation")
                        .WithMany("Pilots")
                        .HasForeignKey("CorporationID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.PilotSkill", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Pilot", "Pilot")
                        .WithMany("PilotSkills")
                        .HasForeignKey("PilotId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Skill", "Skill")
                        .WithMany("PilotSkills")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.SelectedFit", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Fit", "Fit")
                        .WithMany("SelectedFits")
                        .HasForeignKey("FitId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.WaitingPilot", "WaitingPilot")
                        .WithMany("SelectedFits")
                        .HasForeignKey("WaitingPilotId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.SelectedRole", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.FleetRole", "FleetRole")
                        .WithMany("SelectedRoles")
                        .HasForeignKey("FleetRoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.WaitingPilot", "WaitingPilot")
                        .WithMany("SelectedRoles")
                        .HasForeignKey("WaitingPilotId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.ShipSkill", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.ShipType", "ShipType")
                        .WithMany("ShipSkills")
                        .HasForeignKey("ShipTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Skill", "Skill")
                        .WithMany("ShipSkills")
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Imperium_Incursions_Waitlist.Models.WaitingPilot", b =>
                {
                    b.HasOne("Imperium_Incursions_Waitlist.Models.Pilot", "Pilot")
                        .WithMany()
                        .HasForeignKey("PilotId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.Account", "RemovedByAccount")
                        .WithMany("RemovedPilots")
                        .HasForeignKey("RemovedByAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Imperium_Incursions_Waitlist.Models.StarSystem", "System")
                        .WithMany()
                        .HasForeignKey("SystemId");
                });
#pragma warning restore 612, 618
        }
    }
}
