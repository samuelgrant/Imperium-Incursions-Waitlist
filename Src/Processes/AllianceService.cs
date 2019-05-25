using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Imperium_Incursions_Waitlist.Data;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.Extensions.DependencyInjection;
using Imperium_Incursions_Waitlist.Services;

public class AllianceService : IHostedService
{
    private Timer _timer;
    private WaitlistDataContext _Db;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CorporationService> _logger;

    public AllianceService(ILogger<CorporationService> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Create a new scope to retrieve scoped services
        var scope = _serviceProvider.CreateScope();

        // Get the DbContext instance
        _Db = scope.ServiceProvider.GetRequiredService<WaitlistDataContext>();

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
                  TimeSpan.FromHours(1));

    }

    private void DoWork(object state)
    {
        _logger.LogInformation("Background Service Started: updating alliances.");
        List<Alliance> alliances = _Db.Alliance.ToList();
        foreach (Alliance alliance in alliances)
        {
            // Skip the non-membership alliance
            if (alliance.Id == 0)
                continue;

            var result = EsiWrapper.GetAlliance(alliance.Id);
            alliance.Name = result.Result.Name;
        }

        _Db.SaveChanges();
        _logger.LogInformation("Background Service Completed: alliances updated.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}