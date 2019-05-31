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
                // Update fleet location based on the Fleet Boss
                await pilot.UpdateToken();
                var System = await EsiWrapper.GetSystem((AuthorizedCharacterData)pilot);
                fleet.SystemId = System?.SolarSystemId;
                fleet.ErrorCount = null;

                // Update the pilots in fleet
                await pilot.UpdateToken();
                long fleetId = (long)fleet.EveFleetId;
                List<FleetAssignment> current_members = _Db.FleetAssignments.Where(c => c.FleetId == fleet.Id && c.DeletedAt == null).Include( c => c.WaitingPilot).ToList();
                Dictionary<int, FleetAssignment> knownMembers = current_members.ToDictionary(x => x.WaitingPilot.PilotId, x => x);
                List<ESI.NET.Models.Fleets.Member> esiMembers = await EsiWrapper.GetFleetMembers((AuthorizedCharacterData)pilot, fleetId);
                
                if(esiMembers != null)
                {
                    foreach(var fleetMember in esiMembers)
                    {
                        // If we know about the fleet member update their info
                        if (knownMembers.ContainsKey(fleetMember.CharacterId))
                        {
                            knownMembers[fleetMember.CharacterId].CurrentShipId = fleetMember.ShipTypeId;
                            knownMembers[fleetMember.CharacterId].TakesFleetWarp = fleetMember.TakesFleetWarp;
                            knownMembers[fleetMember.CharacterId].UpdatedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            // Otherwise they're new, let's add them to our fleet assignments
                            var waitlistId = CheckWaitlistForPilot(fleetMember.CharacterId, fleet);
                            _Db.FleetAssignments.Add(new FleetAssignment
                            {
                                WaitingPilotId = (waitlistId != null) ? waitlistId : null,
                                FleetId = fleet.Id,
                                IsExitCyno = false,
                                CurrentShipId = fleetMember.ShipTypeId,
                                TakesFleetWarp = fleetMember.TakesFleetWarp,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    // Delete pilots who have not been reported through ESI for more than 2 minutes.
                    current_members = _Db.FleetAssignments.Where(c => c.FleetId == fleet.Id).ToList();
                    foreach(FleetAssignment member in current_members)
                        if (member.UpdatedAt.Value.AddMinutes(1) < DateTime.UtcNow)
                            member.DeletedAt = DateTime.UtcNow;
                    
                }
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

    private int? CheckWaitlistForPilot(int characterId, Fleet fleet)
    {
        WaitingPilot x = _Db.WaitingPilots.Where(c => c.PilotId == characterId && c.RemovedByAccountId == null).FirstOrDefault();

        if(x == null)
        {
            // Create a waiting pilot!
            WaitingPilot waitlistPilot = new WaitingPilot
            {
                RemovedByAccountId = fleet.BossPilot.AccountId,
                PilotId = characterId,
                FleetAssignment = null,
                SystemId = null,
                IsOffline = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _Db.Add(waitlistPilot);
            _Db.SaveChanges();
            //Return that waiting pilot
            return waitlistPilot.Id;
        }

        x.RemovedByAccountId = fleet.BossPilot.AccountId;
        x.UpdatedAt = DateTime.UtcNow;

        return x.Id;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}