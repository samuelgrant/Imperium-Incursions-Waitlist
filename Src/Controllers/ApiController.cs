using System.Linq;
using ESI.NET.Models.SSO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System;

namespace Imperium_Incursions_Waitlist.Controllers.Auth
{
    [Route("/api/")]
    public class ApiController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public ApiController(Data.WaitlistDataContext db, ILogger<ApiController> logger)
        {
            _Db = db;
            _Logger = logger;
        }


        [HttpPost("esi-ui/show-info")]
        public async Task<IActionResult> ShowInfo(IFormCollection request)
        {
            int target_id = request._int("target_id");
            Pilot pilot = await _Db.Pilots.FindAsync(Request.Cookies.PreferredPilotId());
            await pilot.UpdateToken();

            EsiWrapper.ShowInfo((AuthorizedCharacterData)pilot, target_id);

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("esi-ui/destination")]
        public async Task<IActionResult> SetDestination(IFormCollection request)
        {
            int target_id = request._int("target_id");
            Pilot pilot = await _Db.Pilots.FindAsync(Request.Cookies.PreferredPilotId());
            await pilot.UpdateToken();

            EsiWrapper.SetDestination((AuthorizedCharacterData)pilot, target_id);

            await _Db.SaveChangesAsync();

            return Ok();
        }


        [HttpGet("v1/waitlist/pilots")]
        [Produces("application/json")]
        [Authorize(Roles = "Commander,Leadership,Dev")]
        public async Task<IActionResult> GetWaitingPilots()
        {
            var waitlist = await _Db.WaitingPilots
                .Where(c => c.RemovedByAccountId == null)
                .Include(a => a.SelectedRoles)
                .ThenInclude(ac => ac.FleetRole)
                .Include(c => c.SelectedFits)
                .ThenInclude(ac => ac.Fit)
                .Select(c => new {
                    c.Id,
                    c.Pilot,
                    account = new{c.Pilot.AccountId, c.Pilot.Account.Name },
                    c.IsOffline,
                    c.NewPilot,
                    ships = c.SelectedFits.Select(s => new { s.Fit.Id, s.Fit.ShipTypeId, s.Fit.Description }),
                    roles = c.SelectedRoles.Select(s => new { s.FleetRole.Name, s.FleetRole.Acronym }),
                    system = (c.System != null) ? new { c.System.Id, c.System.Name } : null,
                    altInFleet = "",
                    c.WaitingFor,
                }).ToListAsync();


            return Ok(waitlist);
        }

        [Authorize]
        [HttpGet("v1/options")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFcSettings()
        {
            var prefPilot = new {
                id = Request.Cookies.PreferredPilotId(),
                name = Request.Cookies.PreferredPilotName()
            };

            if (User.IsInRole("Commander") || User.IsInRole("Leadership") || User.IsInRole("Dev"))
            {
                var commChannels = await _Db.CommChannels.Select(s => new { s.Id, s.LinkText, s.Url }).ToListAsync();
                List<string> FleetTypes = Enum.GetValues(typeof(FleetType)).Cast<FleetType>()
                                                                           .Select(v => v.ToString())
                                                                           .ToList();

                var bossEligible = await _Db.Pilots.Where(c => c.AccountId == User.AccountId() && c.ESIValid).Select(s => new {
                    id = s.CharacterID,
                    name = s.CharacterName
                }).OrderBy(o => o.name).ToListAsync();

                return Ok(new
                {
                    fcOptions = new {comms = commChannels, fleetTypes = FleetTypes, pilots = bossEligible},
                    prefPilot
                });
            }

            return Ok(new
            {
                fcOptions = new { },
                prefPilot
            });
        }
    }
}