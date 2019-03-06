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
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}