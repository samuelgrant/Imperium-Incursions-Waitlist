using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Imperium_Incursions_Waitlist.Data;

namespace Imperium_Incursions_Waitlist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Remove old log files at 00:00 UTC
            Task.IntervalInDays(00, 00, 1, () => { Log.PurgeOldLogs(); });

            var host = CreateWebHostBuilder(args).Build();

            // Run seeder
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<WaitlistDataContext>();
                    DBInitializer.Initialize(context);
                    Log.Debug("Accounts table seeded.");
                }
                catch(Exception ex)
                {
                    Log.Debug("Error seeding Accounts table. Error: " + ex.Message);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}