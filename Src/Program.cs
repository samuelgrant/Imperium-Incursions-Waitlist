using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Remove old log files at 00:00 UTC
            Task.IntervalInDays(00, 00, 1, () => { Log.PurgeOldLogs(); });

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
