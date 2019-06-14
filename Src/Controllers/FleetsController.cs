using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ESI.NET.Models.SSO;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize(Roles = "Commander,Leadership,Dev")]
    public class FleetsController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public FleetsController(Data.WaitlistDataContext db, ILogger<FleetsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [Route("/fleets/{id}")]
        public IActionResult Index(int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return Redirect("/");

            ViewData["fleetId"] = id;
            return View(viewName: "~/Views/FleetManagement.cshtml");
        }

        [HttpGet]
        [Route("/fleets/{id}/data")]
        [Produces("application/json")]
        public async Task<IActionResult> Data(int id)
        {

            var Fleet = _Db.Fleets.Include(i => i.BossPilot)
                                  .Include(i => i.CommChannel)
                                  .Include(i => i.BackseatAccount)
                                    .ThenInclude(i => i.Pilots)
                                  .Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();

            var fleet1 = await _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).Select(s => new {
                s.Id,
                // Custom properties
                BackseatAccount = s.BackseatAccount == null ? null : new {
                    s.BackseatAccount.Id,
                    s.BackseatAccount.Name
                },
                BossPilot = s.BossPilot == null ? null : new {
                    id = s.BossPilot.CharacterID,
                    name = s.BossPilot.CharacterName
                },
                commChannel = new {
                    s.CommChannel.Id,
                    s.CommChannel.LinkText,
                    s.CommChannel.Url
                },
                system = new {
                    s.System.Id,
                    s.System.Name
                },
                // Used for Fleet at a Glance & Exit Cynos.
                members = new {
                    onGrid = s.GetOngridCount(s.FleetAssignments.ToList()),
                    max = s.GetFleetTypeMax(),
                    pilots = s.FleetAssignments.Select( s1 => new {
                        id = s1.WaitingPilot.Pilot.CharacterID,
                        name = s1.WaitingPilot.Pilot.CharacterName,
                        s1.IsExitCyno,
                        s1.TakesFleetWarp,
                        joinedAt = s1.CreatedAt,
                        ship = new { id = s1.ActiveShip.Id, name = s1.ActiveShip.Name, s1.ActiveShip.Queue },
                        system = new { s1.System.Id, s1.System.Name }
                    })
                },

                // Standard properties
                s.EveFleetId,
                s.IsPublic,
                s.Type,
                s.Wings

            }).FirstOrDefaultAsync();

            if (fleet1 == null)
                return NotFound($"Fleet {id} not found.");


            return Ok(fleet1);
        }

        [HttpPut]
        [Route("/fleets/{id}/backseat")]
        [Produces("application/json")]
        public IActionResult Backseat(int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            Account account = _Db.Accounts.Find(User.AccountId());
            if (account == null)
                return BadRequest("Account not found.");

            try
            {
                fleet.BackseatAccount = account;
                _Db.SaveChanges();
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("/fleets/{id}/backseat")]
        [Produces("application/json")]
        public IActionResult ClearBackseat(int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.BackseatAccount = null;
                _Db.SaveChanges();

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
        /// <param name="request"></param>
        /// <param name="id"></param>
        [HttpPut]
        [Route("/fleets/{id}/boss")]
        [Produces("application/json")]
        public IActionResult Boss(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            int bossId = int.Parse(request["pilotId"]);
            var pilot = _Db.Pilots.Where(c => c.CharacterID == bossId && c.AccountId == int.Parse(User.FindFirst("Id").Value)).FirstOrDefault();
            if (pilot == null)
                return BadRequest("The pilot was not found, or you do not have permission to complete this request.");


            try
            {
                fleet.BossPilot = pilot;
                _Db.SaveChanges();

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
        /// <param name="request"></param>
        /// <param name="id"></param>
        [HttpPut]
        [Route("/fleets/{id}/comms")]
        [Produces("application/json")]
        public IActionResult Comms(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            int commsId = int.Parse(request["commsId"].ToString());

            CommChannel comm = _Db.CommChannels.Find(commsId);
            if (comm == null)
                return BadRequest("Comms setting is invalid");

            try
            {
                fleet.CommChannel = comm;
                _Db.SaveChanges();

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
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/fleets/{id}/status")]
        [Produces("application/json")]
        public IActionResult Status(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.IsPublic = bool.Parse(request["status"].ToString());
                _Db.SaveChanges();

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
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/fleets/{id}/type")]
        [Produces("application/json")]
        public IActionResult Type(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            try
            {
                fleet.Type = request["type"].ToString();
                _Db.SaveChanges();

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
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(IFormCollection request)
        {

            string EsiUrl = request["EsiFleetUrl"].ToString();
            long fleetId;
            try
            {
                fleetId = request["EsiFleetUrl"].ToString().GetEsiId();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Cannot parse the ESI Fleet ID from the URL provided. {0}", EsiUrl);
                return BadRequest(string.Format("Cannot parse the ESI Fleet ID from the URL provided. {0}\n{1}", EsiUrl, ex.Message));
            }

            int bossId = int.Parse(request["fleetBoss"].ToString());
            Pilot pilot = _Db.Pilots.Where(c => c.CharacterID == bossId  && c.AccountId == int.Parse(User.FindFirst("Id").Value)).FirstOrDefault();
            if (pilot == null)
                return NotFound("Pilot not found, or you do not have access to it.");

            string fleetType = request["FleetType"].ToString();

            //Is there an active fleet with this ID? IF yes redirect to that fleet else continue
            var fleet = _Db.Fleets.Where(c => c.EveFleetId == fleetId && c.ClosedAt == null).FirstOrDefault();

            if (fleet != null)
                // Fleet already registered let's redirect the user to that page.
                return Ok(fleet.Id);

            Models.CommChannel comms = _Db.CommChannels.Find(int.Parse(request["FleetComms"].ToString()));
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

            _Db.Fleets.Add(newFleet);
            _Db.SaveChanges();

            // Redirect to page!
            return Ok(newFleet.Id);
        }

        [HttpDelete]
        [Route("/fleets/{id}")]
        [Produces("application/json")]
        public IActionResult Close(int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            fleet.ClosedAt = DateTime.UtcNow;

            _Db.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("/fleets/{fleetId}/invite/{pilotId}")]
        public async Task<IActionResult> Invite(int fleetId, int pilotId, IFormCollection request)
        {
            Fleet fleet = _Db.Fleets.Where(c => c.Id == fleetId).Include(c => c.BossPilot).FirstOrDefault();
            if (fleet == null)
                return BadRequest("The fleet was not found");

            Pilot boss = _Db.Pilots.Find(fleet.BossPilotId);
            if (boss == null)
                return BadRequest("The fleet boss was not found");

            if (!boss.ESIValid)
                return Unauthorized("Could not validate the FCs ESI Tokens");

            await boss.UpdateToken();
            _Db.SaveChanges();

            try
            {
                long.TryParse(request["squadId"].ToString(), out long squad_id);
                long.TryParse(request["wingId"].ToString(), out long wing_id);
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

        [HttpPost]
        [Produces("application/json")]
        [Route("/fleets/{fleetId}/alarm/{accountId}")]
        public HttpResponseMessage Alarm(int fleetId, int accountId)
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }

        [HttpPut("/fleets/{fleetId}/cyno/{pilotId}")]
        [Produces("application/json")]
        public IActionResult Cyno(int fleetId, int pilotId)
        {
            FleetAssignment pilot = _Db.FleetAssignments.Where(c => c.FleetId == fleetId && c.WaitingPilot.PilotId == pilotId).FirstOrDefault();
            if (pilot == null)
                return NotFound("The pilot was not found.");

            try
            {
                pilot.IsExitCyno = !pilot.IsExitCyno;
                _Db.SaveChanges();
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