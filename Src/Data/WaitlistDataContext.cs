using Microsoft.EntityFrameworkCore;
using Imperium_Incursions_Waitlist.Models;

namespace Imperium_Incursions_Waitlist.Data
{
    public class WaitlistDataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Corporation> Corporation { get; set; }
        public DbSet<Alliance> Alliance { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<PilotSkill> PilotSkills { get; set; }
        public DbSet<ShipType> ShipTypes { get; set; }
        public DbSet<ShipSkill> ShipSkills { get; set; }
        public DbSet<Fit> Fits { get; set; }
        public DbSet<CommChannel> CommChannels { get; set; }
        public DbSet<Fleet> Fleets { get; set; }
        public DbSet<FleetAssignment> FleetAssignments { get; set; }
        public DbSet<FleetRole> FleetRoles { get; set; }
        public DbSet<SelectedFit> SelectedFits { get; set; }
        public DbSet<SelectedRole> SelectedRoles { get; set; }
        public DbSet<WaitingPilot> WaitingPilots { get; set; }

        public WaitlistDataContext(DbContextOptions<WaitlistDataContext> options) : base(options)
        {
            // Not needed as we are using migrations(?)
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            // Configure account to ban relationships

            modelBuilder.Entity<Ban>()
                        .HasOne<Account>("BannedAccount")
                        .WithMany("AccountBans")
                        .HasForeignKey("BannedAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ban>()
                        .HasOne<Account>("UpdatingAdmin")
                        .WithMany("UpdatedBans")
                        .HasForeignKey("UpdatedByAdminId")
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ban>()
                        .HasOne<Account>("CreatorAdmin")
                        .WithMany("CreatedBans")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict);

            // Configure account to note relationships

            modelBuilder.Entity<Note>()
                        .HasOne<Account>("TargetAccount")
                        .WithMany("AccountNotes")
                        .HasForeignKey("TargetAccountId")
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Note>()
                        .HasOne<Account>("UpdatingAdmin")
                        .WithMany("UpdatedNotes")
                        .HasForeignKey("UpdatedByAdminId")
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Note>()
                        .HasOne<Account>("CreatorAdmin")
                        .WithMany("CreatedNotes")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Pilot>()
                    .HasOne<Corporation>("Corporation")
                    .WithMany("Pilots")
                    .HasForeignKey("CorporationID")
                    .OnDelete(DeleteBehavior.Restrict);

            // Configuring account roles m-m relationship

            modelBuilder.Entity<AccountRole>()
                        .HasKey(ar => new { ar.AccountId, ar.RoleId });
            modelBuilder.Entity<AccountRole>()
                        .HasOne(ar => ar.Account)
                        .WithMany(a => a.AccountRoles)
                        .HasForeignKey(ar => ar.AccountId);
            modelBuilder.Entity<AccountRole>()
                        .HasOne(ar => ar.Role)
                        .WithMany(r => r.AccountRoles)
                        .HasForeignKey(ar => ar.RoleId);

            // Finished configuring account roles relationship

            // Configuring fleet - waiting pilot relationship
            modelBuilder.Entity<FleetAssignment>()
                .HasKey(fa => new { fa.WaitingPilotId, fa.FleetId });
            modelBuilder.Entity<FleetAssignment>()
                .HasOne(fa => fa.Fleet)
                .WithMany(f => f.FleetAssignments)
                .HasForeignKey(fa => fa.FleetId);
            modelBuilder.Entity<FleetAssignment>()
                .HasOne(fa => fa.WaitingPilot)
                .WithOne(wp => wp.FleetAssignment);

            // Configuring m-m relationship between waiting pilot and fits
            modelBuilder.Entity<SelectedFit>()
                .HasKey(sf => new { sf.WaitingPilotId, sf.FitId });
            modelBuilder.Entity<SelectedFit>()
                .HasOne(sf => sf.Fit)
                .WithMany(f => f.SelectedFits)
                .HasForeignKey(sf => sf.FitId);
            modelBuilder.Entity<SelectedFit>()
                .HasOne(sf => sf.WaitingPilot)
                .WithMany(wp => wp.SelectedFits)
                .HasForeignKey(sf => sf.WaitingPilotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuring m-m relation between waiting pilot and fleet roles
            modelBuilder.Entity<SelectedRole>()
                .HasKey(sr => new { sr.WaitingPilotId, sr.FleetRoleId });
            modelBuilder.Entity<SelectedRole>()
                .HasOne(sr => sr.FleetRole)
                .WithMany(fr => fr.SelectedRoles)
                .HasForeignKey(sr => sr.FleetRoleId);
            modelBuilder.Entity<SelectedRole>()
                .HasOne(sr => sr.WaitingPilot)
                .WithMany(wp => wp.SelectedRoles)
                .HasForeignKey(sr => sr.WaitingPilotId);

            // Configuring m-m relationship between pilot and skill
            modelBuilder.Entity<PilotSkill>()
                .HasKey(ps => new { ps.PilotId, ps.SkillId });
            modelBuilder.Entity<PilotSkill>()
                .HasOne(ps => ps.Pilot)
                .WithMany(p => p.PilotSkills)
                .HasForeignKey(ps => ps.PilotId);
            modelBuilder.Entity<PilotSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.PilotSkills)
                .HasForeignKey(ps => ps.SkillId);

            // Configuring m-m relationship between ship type and skill
            modelBuilder.Entity<ShipSkill>()
                .HasKey(ss => new { ss.ShipTypeId, ss.SkillId });
            modelBuilder.Entity<ShipSkill>()
                .HasOne(ss => ss.ShipType)
                .WithMany(st => st.ShipSkills)
                .HasForeignKey(ss => ss.ShipTypeId);
            modelBuilder.Entity<ShipSkill>()
                .HasOne(ss => ss.Skill)
                .WithMany(s => s.ShipSkills)
                .HasForeignKey(ss => ss.SkillId);

            modelBuilder.Entity<WaitingPilot>()
                .HasOne(wp => wp.RemovedByAccount)
                .WithMany(rba => rba.RemovedPilots)
                .HasForeignKey(wp => wp.RemovedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeding account roles

            modelBuilder.Entity<Role>()
                        .HasData(
                            new Role { Id = 1, Name = "Commander" },
                            new Role { Id = 2, Name = "Leadership"}
                        );

            modelBuilder.Entity<Alliance>()
                        .HasData(new { Id = 0, Name = "" });

            // Finished seeding account roles
        }
    }
}