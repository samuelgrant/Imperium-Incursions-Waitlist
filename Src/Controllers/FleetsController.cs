using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ESI.NET.Models.SSO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize(Roles = "Commander,Leadership,Dev")]
    [Route("/fleets/{fleetId}")]
    public class FleetsController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public FleetsController(Data.WaitlistDataContext db, ILogger<FleetsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return Redirect("/");

            ViewData["fleetId"] = fleetId;
            return View(viewName: "~/Views/FleetManagement.cshtml");
        }

        [HttpGet("data")]
        [Produces("application/json")]
        public async Task<IActionResult> Data(int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).Select(s => new {
                s.Id,
                // Custom properties
                BackseatAccount = s.BackseatAccount == null ? null : new
                {
                    s.BackseatAccount.Id,
                    s.BackseatAccount.Name
                },
                BossPilot = s.BossPilot == null ? null : new
                {
                    id = s.BossPilot.CharacterID,
                    name = s.BossPilot.CharacterName
                },
                commChannel = new
                {
                    s.CommChannel.Id,
                    s.CommChannel.LinkText,
                    s.CommChannel.Url
                },
                system = s.System == null ? null : new {
                    s.System.Id,
                    s.System.Name
                },
                // Used for Fleet at a Glance & Exit Cynos.
                members = new
                {
                    onGrid = s.GetOngridCount(s.FleetAssignments.ToList()),
                    max = s.GetFleetTypeMax(),
                    pilots = s.FleetAssignments.Where(c => c.DeletedAt == null).Select(s1 => new
                    {
                        id = s1.WaitingPilot.Pilot.CharacterID,
                        name = s1.WaitingPilot.Pilot.CharacterName,
                        s1.IsExitCyno,
                        s1.TakesFleetWarp,
                        joinedAt = s1.CreatedAt,
                        ship = new { id = s1.ActiveShip.Id, name = s1.ActiveShip.Name, queue = new { id = s1.ActiveShip.Queue, name = s1.ActiveShip.Queue.ToString() } },
                        system = new { s1.System.Id, s1.System.Name }
                    })
                },

                // Standard properties
                s.EveFleetId,
                s.IsPublic,
                s.Type,
                s.Wings

            }).FirstOrDefaultAsync();

            if (fleet == null)
                return NotFound($"Fleet {fleetId} not found.");


            return Ok(fleet);
        }

        [HttpPut("backseat")]
        [Produces("application/json")]
        public async Task<IActionResult> Backseat(int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            Account account = await _Db.Accounts.FindAsync(User.AccountId());
            if (account == null)
                return BadRequest("Account not found.");

            try
            {
                fleet.BackseatAccount = account;
                await _Db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot set the fleet backseat (Fleet ID: {0}) {1} {2}", fleet.Id, account.Name, ex.Message);
                return BadRequest("Error setting the backseat.");
            }
            
        }

        /// <summary>
        /// Unsets the backseat FC
        /// </summary>
        [HttpDelete("backseat")]
        [Produces("application/json")]
        public async Task<IActionResult> ClearBackseat(int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.BackseatAccount = null;
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot clear the fleet backseat (Fleet ID: {0})  {1}.", fleet.Id, ex.Message);
                return BadRequest("Error clearing the backseat.");
            }
        }



        /// <summary>
        /// Sets the fleet boss (FC with star in game)
        /// </summary>
        [HttpPut("boss")]
        [Produces("application/json")]
        public async Task<IActionResult> Boss(IFormCollection request, int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            int bossId = request._int("pilotId");
            var pilot = await _Db.Pilots.Where(c => c.CharacterID == bossId && c.AccountId == User.AccountId()).FirstOrDefaultAsync();
            if (pilot == null)
                return BadRequest("The pilot was not found, or you do not have permission to complete this request.");


            try
            {
                fleet.BossPilot = pilot;
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot change the fleet boss (Fleet ID: {0}) to {1} {2}.", fleet.Id, pilot.CharacterName, ex.Message);
                return BadRequest("Error setting fleet boss.");
            }
        }

        /// <summary>
        /// Sets the fleet comms.
        /// </summary>
        [HttpPut("comms")]
        [Produces("application/json")]
        public async Task<IActionResult> Comms(IFormCollection request, int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            int commsId = request._int("commsId");

            CommChannel comm = await _Db.CommChannels.FindAsync(commsId);
            if (comm == null)
                return BadRequest("Comms setting is invalid");

            try
            {
                fleet.CommChannel = comm;
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot change the fleet comms (Fleet ID: {0}) channel {1} {2}.", fleet.Id, comm.LinkText, ex.Message);
                return BadRequest("Error setting fleet comms.");
            }
        }

        /// <summary>
        /// Sets the fleet status.
        /// </summary>
        [HttpPut("status")]
        [Produces("application/json")]
        public async Task<IActionResult> Status(IFormCollection request, int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.IsPublic = bool.Parse(request._str("status"));
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot change the fleet status (Fleet ID: {0}) Status: {1} {2}.", fleet.Id, fleet.IsPublic, ex.Message);
                return BadRequest("Error setting fleet status.");
            }
        }

        /// <summary>
        /// Sets the fleet type.
        /// </summary>
        [HttpPut("type")]
        [Produces("application/json")]
        public async Task<IActionResult> Type(IFormCollection request, int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.Type = request._str("type");
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot change the fleet type (Fleet ID: {0}) Type: {1} {2}.", fleet.Id, fleet.Type, ex.Message);
                return BadRequest("Error setting fleet type.");
            }
        }

        /// <summary>
        /// Registers a new fleet with the waitlist
        /// </summary>
        /// <!--TODO: Fleet Ownership Check! -->
        [HttpPost("/fleets")]
        public async Task<IActionResult> Index(IFormCollection request)
        {

            string EsiUrl = request._str("EsiFleetUrl");
            long fleetId;

            try
            {
                fleetId = request._str("EsiFleetUrl").GetEsiId();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot parse the ESI Fleet ID from the URL provided. {0}", EsiUrl);
                return BadRequest(string.Format("Cannot parse the ESI Fleet ID from the URL provided. {0}\n{1}", EsiUrl, ex.Message));
            }

            int bossId = request._int("fleetBoss");
            Pilot pilot = await _Db.Pilots.Where(c => c.CharacterID == bossId  && c.AccountId == User.AccountId()).FirstOrDefaultAsync();
            if (pilot == null)
                return NotFound("Pilot not found, or you do not have access to it.");

            string fleetType = request._str("FleetType");

            //Is there an active fleet with this ID? IF yes redirect to that fleet else continue
            var fleet = await _Db.Fleets.Where(c => c.EveFleetId == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();

            if (fleet != null)
                // Fleet already registered let's redirect the user to that page.
                return Ok(fleet.Id);

            CommChannel comms = await _Db.CommChannels.FindAsync(request._int("FleetComms"));
            if (comms == null)
                // Fleet comms not found
                _Logger.LogError("Invalid Comms channel provided.");


            // TODO: Can we actually ESI into this fleet?
            if (false)
            {
                _Logger.LogWarning("(ID: {0}) does not have access to fleet {1}.", bossId, fleetId);
                return Forbid("Pilot selected does not have access to that fleet. Check you selected the correct fleet boss!");
            }

            Fleet newFleet = new Fleet
            {
                EveFleetId = fleetId,
                BossPilot = pilot,
                CommChannelId = comms.Id,
                IsPublic = false,
                Type = fleetType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _Db.AddAsync(newFleet);
            await _Db.SaveChangesAsync();

            // Redirect to page!
            return Ok(newFleet.Id);
        }

        [HttpDelete("")]
        [Produces("application/json")]
        public async Task<IActionResult> Close(int fleetId)
        {
            var fleet = await _Db.Fleets.Where(c => c.Id == fleetId && c.ClosedAt == null).FirstOrDefaultAsync();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            fleet.ClosedAt = DateTime.UtcNow;

            List<FleetAssignment> pilots = await _Db.FleetAssignments.Where(c => c.FleetId == fleet.Id && c.DeletedAt == null).ToListAsync();
            foreach (FleetAssignment pilot in pilots)
                pilot.DeletedAt = DateTime.UtcNow;

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("invite/{pilotId}")]
        [Produces("application/json")]
        public async Task<IActionResult> Invite(int fleetId, int pilotId, IFormCollection request)
        {
            Fleet fleet = await _Db.Fleets.Where(c => c.Id == fleetId).Include(c => c.BossPilot).FirstOrDefaultAsync();
            if (fleet == null)
                return BadRequest("The fleet was not found");

            Pilot boss = await _Db.Pilots.FindAsync(fleet.BossPilotId);
            if (boss == null)
                return BadRequest("The fleet boss was not found");

            if (!boss.ESIValid)
                return Unauthorized("Could not validate the FCs ESI Tokens");

            await boss.UpdateToken();
            await _Db.SaveChangesAsync();

            try
            {
                long.TryParse(request._str("squadId"), out long squad_id);
                long.TryParse(request._str("wingId"), out long wing_id);

                DefaultSquad squadPosition;
                if (squad_id == 0)
                {
                    squadPosition = fleet.DefaultSquad();
                }
                else
                {
                    squadPosition = new DefaultSquad
                    {
                        squad_id = squad_id,
                        wing_id = wing_id
                    };
                }
                
                var x = EsiWrapper.FleetInvite((AuthorizedCharacterData)boss, fleet.EveFleetId, squadPosition.squad_id, squadPosition.wing_id, pilotId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("alarm/{accountId}")]
        [Produces("application/json")]
        public HttpResponseMessage Alarm(int fleetId, int accountId)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }

        [HttpPut("cyno/{pilotId}")]
        [Produces("application/json")]
        public async Task<IActionResult> Cyno(int fleetId, int pilotId)
        {
            FleetAssignment pilot = await _Db.FleetAssignments.Where(c => c.FleetId == fleetId && c.WaitingPilot.PilotId == pilotId).FirstOrDefaultAsync();
            if (pilot == null)
                return NotFound("The pilot was not found.");

            try
            {
                pilot.IsExitCyno = !pilot.IsExitCyno;
                await _Db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error updating {0} status as an exit cyno: {1}", pilot.WaitingPilot.Pilot.CharacterName, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}