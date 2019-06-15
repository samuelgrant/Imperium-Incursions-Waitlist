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
using System.ComponentModel;
using Imperium_Incursions_Waitlist;

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
        _logger.LogInformation("Background Service Started: updating fleets.");
        List<Fleet> fleets = _Db.Fleets.Include(ci => ci.BossPilot).Where(c => c.BossPilot != null && c.ClosedAt == null).ToList();
        if (fleets.Count == 0) return;

        foreach (Fleet fleet in fleets)
        {
            //I'm not sure why, but occasionally a fleet without a boss will bleed through
            //This happens if boss was null, and then updated by an FC before this system next runs.
            if (fleet.BossPilotId == null)
                continue;

            Pilot pilot = _Db.Pilots.Find(fleet.BossPilotId);

            try
            {
                // Update fleet location based on the Fleet Boss
                await pilot.UpdateToken();
                if (!pilot.ESIValid)
                {
                    throw new UnauthorizedAccessException("The Fleet Boss's ESI token is no longer valid");
                }

                var System = await EsiWrapper.GetSystem((AuthorizedCharacterData)pilot);
                fleet.SystemId = System?.SolarSystemId;

                // Update Wings, Squads
                await pilot.UpdateToken();
                fleet.Wings = await EsiWrapper.GetFleetLayout((AuthorizedCharacterData)pilot, fleet.EveFleetId);

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
                            knownMembers[fleetMember.CharacterId].ShipTypeId = fleetMember.ShipTypeId;
                            knownMembers[fleetMember.CharacterId].TakesFleetWarp = fleetMember.TakesFleetWarp;
                            knownMembers[fleetMember.CharacterId].UpdatedAt = DateTime.UtcNow;
                            knownMembers[fleetMember.CharacterId].SystemId = fleetMember.SolarSystemId;
                        }
                        else
                        {
                            // Otherwise they're new, let's add them to our fleet assignments
                            var waitlistId = await CheckWaitlistForPilot(fleetMember.CharacterId, fleet);
                            _Db.FleetAssignments.Add(new FleetAssignment
                            {
                                WaitingPilotId = waitlistId ?? null,
                                FleetId = fleet.Id,
                                IsExitCyno = false,
                                ShipTypeId = fleetMember.ShipTypeId,
                                TakesFleetWarp = fleetMember.TakesFleetWarp,
                                SystemId = fleetMember.SolarSystemId,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    // Delete pilots who have not been reported through ESI for more than 1 minutes.
                    current_members = _Db.FleetAssignments.Where(c => c.FleetId == fleet.Id && c.DeletedAt == null).ToList();
                    foreach(FleetAssignment member in current_members)
                        if (member.UpdatedAt.Value.AddMinutes(1) < DateTime.UtcNow)
                            member.DeletedAt = DateTime.UtcNow;

                    await _Db.SaveChangesAsync();
                }

                // NO errors, resetting counter
                fleet.ErrorCount = null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating fleet {0} (FC: {1}). {2} ", fleet.Id, fleet.BossPilot.CharacterName, ex.Message);

                if (ex.Message == FleetErrorType.FleetDead.ToString())
                {
                    fleet.ClosedAt = DateTime.UtcNow;
                    _logger.LogInformation("The fleet no longer exists and has been closed");
                    continue;
                }

                // Increase error counter
                int? errors = fleet.ErrorCount;
                fleet.ErrorCount = (errors != null) ? errors + 1 : 1;
                

                // Too many errors, deleting fleet boss to protect against error throttling
                if (errors >= 15)
                {
                    fleet.BossPilotId = null;
                    fleet.ErrorCount = null;
                }
            }

            // Touch Updated At timestamp
            fleet.UpdatedAt = DateTime.UtcNow;
        }

        _Db.SaveChanges();
        _logger.LogInformation("Background Service Completed: fleets updated.");
    }

    private async Task<int?> CheckWaitlistForPilot(int characterId, Fleet fleet)
    {
        WaitingPilot x = _Db.WaitingPilots.Where(c => c.PilotId == characterId && c.RemovedByAccountId == null).FirstOrDefault();

        if(x == null)
        {
            Pilot pilot = await _Db.Pilots.FindAsync(characterId);
            if(pilot == null)
            {
                // We don't know who this pilot is, let's look them up with ESI and create some records for them
                var pilotInfo = await EsiWrapper.PilotLookupAsync(characterId);
                Corporation.EnsureInDatabase(pilotInfo.CorporationId, _Db);

                _Db.Add(new Pilot
                {
                    Account = null,
                    CharacterID = characterId,
                    CharacterName = pilotInfo.Name,
                    CorporationID = pilotInfo.CorporationId,
                    RegisteredAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });                
            }

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