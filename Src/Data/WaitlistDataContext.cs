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

            // Todo: Check whether this is needed - I think we don't need to declare this
            // relationship in fluent API
            modelBuilder.Entity<Pilot>()
                    .HasOne<Corporation>("Corporation")
                    .WithMany("Pilots")
                    .HasForeignKey("CorporationId")
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