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
using Microsoft.EntityFrameworkCore;

public class FleetService : IHostedService
{
    private Timer _timer;
    private WaitlistDataContext _Db;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FleetService> _logger;

    public FleetService(ILogger<FleetService> logger, IServiceProvider serviceProvider)
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
                  TimeSpan.FromSeconds(20));

    }

    private async void DoWork(object state)
    {
        _logger.LogInformation("Background Service Started: updating fleets.");
        List<Fleet> fleets = _Db.Fleets.Where(c => c.BossPilot != null && c.ClosedAt == null).Include(ci => ci.BossPilot).ToList();
        foreach (Fleet fleet in fleets)
        {
            Pilot pilot = _Db.Pilots.Find(fleet.BossPilot.CharacterID);

            try
            {
                await pilot.UpdateToken();
                var System = await EsiWrapper.GetSystem((AuthorizedCharacterData)pilot);
                fleet.SystemId = System?.SolarSystemId;
                fleet.ErrorCount = null;


                // Update fleet assignments table
                    // Add new members
                        // Remove pilots in fleet from Waitlist
                    // Remove (soft delete) members who've left for more than two minutes
            }
            catch (Exception ex)
            {
                // Increase error counter
                int? errors = fleet.ErrorCount;
                fleet.ErrorCount = (errors != null) ? errors + 1 : 1;



                // Too many errors, deleting fleet boss to protect against error throttling
                if (errors >= 15)
                {
                    fleet.BossPilotId = null;
                    //_logger.LogWarning("Too many errors have occurred for fleet {0} (FC: {1}. The boss has been removed and ESI queries for this fleet disabled.", 
                    //    fleet.Id, fleet.BossPilot.CharacterName, ex.Message);
                }
                    

                _logger.LogError("Error updating fleet {0} (FC: {1}). {2} ", fleet.Id, fleet.BossPilot.CharacterName, ex.Message);
            }

            // Touch Updated At timestamp
            fleet.UpdatedAt = DateTime.UtcNow;
        }

        _Db.SaveChanges();
        _logger.LogInformation("Background Service Completed: fleets updated.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}