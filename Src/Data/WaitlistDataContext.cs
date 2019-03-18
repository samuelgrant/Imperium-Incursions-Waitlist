using Microsoft.EntityFrameworkCore;
using Imperium_Incursions_Waitlist.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Data
{
    public class WaitlistDataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Pilot> Pilots { get; set; }

        public WaitlistDataContext(DbContextOptions<WaitlistDataContext> options) : base(options)
        {
            // Not needed as we are using migrations(?)
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

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
        }
    }
}