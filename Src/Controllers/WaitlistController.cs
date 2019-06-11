
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Imperium_Incursions_Waitlist.Data;
using System.Net;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    [Route("/waitlist")]
    public class WaitlistController : Controller
    {
        private readonly WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public WaitlistController(Data.WaitlistDataContext db, ILogger<CommandersController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/Index.cshtml");
        }

        [NonAction]
        public bool ValidFleets(Fleet x)
        {
            if(User.IsInRole("Commander") || User.IsInRole("Leadership") || User.IsInRole("Dev"))
                return x.ClosedAt == null;

            return x.ClosedAt == null && x.IsPublic;
        }

        [HttpGet("data")]
        [Produces("application/json")]
        public async Task<IActionResult> Data()
        {
            // Fleets avaliable to the user -> FCs, Leadership & Devs get all open fleets. Pilots get all visible open fleets
            var fleets = await _Db.Fleets.Where(c => ValidFleets(c)).Select(s => new {
                s.Id,
                s.Type,
                Members = new { onGrid = s.GetOngridCount(s.FleetAssignments.ToList()), max = s.GetFleetTypeMax() },
                comms = new { s.CommChannel.LinkText, s.CommChannel.Url },
                fc = s.BossPilot != null ? new { id = s.BossPilot.CharacterID, name = s.BossPilot.CharacterName } : null,
                system = s.System != null ? new { s.System.Id, s.System.Name } : null
            }).ToListAsync();

            // Roles that a user can select as well as their active fits
            var options = new
            {
                roles = await _Db.FleetRoles.Where(c => c.Avaliable).Select(s => new
                {
                    s.Id,
                    s.Name
                }).OrderBy(o => o.Name).ToArrayAsync(),

                fittings = await _Db.Fits.Where(c => c.AccountId == User.AccountId() && !c.IsShipScan && c.DeletedAt == null).Select(s => new
                {
                    s.Id,
                    s.Description,
                    typeId = s.ShipTypeId
                }).ToListAsync()
            };

            // Gets a list of pilots and puts then into one of two arrays: Avaliable or Waiting
            var pilots = new
            {
                waiting = await _Db.WaitingPilots.Where(c => c.Pilot.AccountId == User.AccountId() && c.RemovedByAccount == null).Select(s => new {
                    id = s.Pilot.CharacterID,
                    name = s.Pilot.CharacterName
                }).OrderBy(o => o.name).ToListAsync(),

                avaliable = await _Db.Pilots.Where(c => c.AccountId == User.AccountId() && c.ESIValid).Select(s => new {
                    id = s.CharacterID,
                    name = s.CharacterName
                }).OrderBy(o => o.name).ToListAsync()
            };

            return Ok(new
            {
                fleets,
                pilots,
                options
            });
        }

        [HttpDelete("/")]
        [Produces("application/json")]
        public IActionResult Leave(IFormCollection request)
        {
            List<WaitingPilot> waitingPilots;
            if (request["pilot_id"].ToString() == "")
            {
                waitingPilots = _Db.WaitingPilots.Include(c => c.Pilot).Where(c => c.Pilot.AccountId == User.AccountId() && c.RemovedByAccount == null).ToList();
            }
            else
            {
                waitingPilots = _Db.WaitingPilots.Where(c => c.PilotId == int.Parse(request["pilot_id"].ToString()) && c.RemovedByAccount == null).ToList();
            }

            foreach (WaitingPilot pilot in waitingPilots)
            {
                pilot.RemovedByAccountId = User.AccountId();
            }

            _Db.SaveChanges();

            return Ok();
        }

        [HttpPost("/")]
        [Produces("application/json")]
        public IActionResult Join(IFormCollection request)
        {
            int pilotId = request["pilot_id"].ToString() == "" ? Request.Cookies.PreferredPilotId() : int.Parse(request["pilot_id"].ToString());
            List<int> roleIds = request["role_ids"].ToString().Split(',').Select(item => int.Parse(item)).ToList();
            List<int> fitIds = request["fit_ids"].ToString().Split(',').Select(item => int.Parse(item)).ToList();

            Pilot pilot = _Db.Pilots.Find(pilotId);
            if (pilot == null)
                return NotFound("Pilot not found");

            if (fitIds.Count == 0)
                return BadRequest("You must select a fit before you can join the waitlist");

            try
            {
                WaitingPilot waitlist = new WaitingPilot
                {
                    PilotId = pilot.CharacterID,
                    SelectedFits = null,
                    SelectedRoles = null,
                    RemovedByAccountId = null,
                    NewPilot = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _Db.Add(waitlist);

                foreach (int id in fitIds)
                {
                    _Db.Add(new SelectedFit
                    {
                        FitId = id,
                        WaitingPilotId = waitlist.Id
                    });
                }

                // Add Roles
                foreach (int id in roleIds)
                {
                    _Db.Add(new SelectedRole
                    {
                        FleetRoleId = id,
                        WaitingPilotId = waitlist.Id
                    });
                }
                _Db.SaveChanges();

                return Ok();
            } catch (Exception ex)
            {
                _Logger.LogWarning("{0} could not be added to the waitlist: {1}", pilot.CharacterName, ex.Message);
                return BadRequest($"Could not add {pilot.CharacterName} to the waitlist {ex.Message}");
            }
        }

        [HttpDelete("remove/{id:int}")]
        [Produces("application/json")]
        [Authorize(Roles = "Commander,Leadership,Dev")]
        public IActionResult Remove(int id)
        {
            WaitingPilot pilot = _Db.WaitingPilots.Find(id);
            if (pilot == null)
                return NotFound("The pilot has already either been removed, or invited.");

            pilot.RemovedByAccountId = User.AccountId();
            pilot.UpdatedAt = DateTime.UtcNow;
            _Db.SaveChanges();

            return Ok();
        }

        [HttpPost("clear")]
        [Produces("application/json")]
        [Authorize(Roles ="Commander,Leadership,Dev")]
        public async Task<IActionResult> Clear()
        {
            List<WaitingPilot> waitlist = await _Db.WaitingPilots.Where(c => c.RemovedByAccountId == null).ToListAsync();
            try
            {
                _Logger.LogInformation("{0} is clearing the waitlist.", User.FindFirst("name").ToString());
                foreach (WaitingPilot pilot in waitlist)
                {
                    pilot.RemovedByAccountId = User.AccountId();
                    await _Db.SaveChangesAsync();
                }

                return Ok();
            }
            catch(Exception ex)
            {
                _Logger.LogError("Error clearing the waitlist: {0}", ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}