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
using ESI.NET.Models.SSO;

public class WaitlistService : IHostedService
{
    private Timer _timer;
    private WaitlistDataContext _Db;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WaitlistService> _logger;

    public WaitlistService(ILogger<WaitlistService> logger, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task StartAsync(CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        // Create a new scope to retrieve scoped services
        var scope = _serviceProvider.CreateScope();

        // Get the DbContext instance
        _Db = scope.ServiceProvider.GetRequiredService<WaitlistDataContext>();

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
                  TimeSpan.FromSeconds(20));

    }

    private async void DoWork(object state)
    {
        _logger.LogInformation("Background Service Started: updating waitlist.");

        List<WaitingPilot> waitlist = _Db.WaitingPilots.Where(c => c.RemovedByAccount == null).ToList();
        foreach (WaitingPilot waiting_pilot in waitlist)
        {
            Pilot pilot = _Db.Pilots.Find(waiting_pilot.PilotId);

            // Update pilot system
            await pilot.UpdateToken();
            var System = await EsiWrapper.GetSystem((AuthorizedCharacterData)pilot);
            waiting_pilot.SystemId = System?.SolarSystemId;

            // Update pilot online/offline
            await pilot.UpdateToken();
            waiting_pilot.IsOffline = !await EsiWrapper.IsOnlineAsync((AuthorizedCharacterData)pilot);

            // Touch the updated at timestamp
            waiting_pilot.UpdatedAt = DateTime.UtcNow;
        }

        _Db.SaveChanges();
        _logger.LogInformation("Background Service Completed: waitlist updated.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}