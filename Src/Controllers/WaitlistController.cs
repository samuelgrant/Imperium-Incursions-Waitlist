
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

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
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
        public IActionResult Index()
        {
            return View(viewName: "~/Views/Index.cshtml");
        }

        [HttpGet]
        [Produces("application/json")]
        public IActionResult Fleets()
        {
            try
            {
                if (User.IsInRole("Commander") || User.IsInRole("Leadership"))
                {
                    return Ok(_Db.Fleets.Where(c => c.ClosedAt == null)
                        .Include(c => c.FleetAssignments)
                        .Select(c => new
                        {
                            c.Id,
                            c.Type,
                            Members = new { onGrid = c.GetOngridCount(c.FleetAssignments.ToList()), max = c.GetFleetTypeMax() },
                            comms = new { c.CommChannel.LinkText, c.CommChannel.Url },
                            fc = (c.BossPilot != null) ? new { c.BossPilot.CharacterID, c.BossPilot.CharacterName } : null,
                            system = (c.System != null) ? new { c.System.Id, c.System.Name } : null
                        }).ToList());
                }
                else
                {
                    return Ok(_Db.Fleets.Where(c => c.ClosedAt == null && c.IsPublic)
                        .Include(c => c.FleetAssignments)
                        .Select(c => new
                        {
                            c.Id,
                            c.Type,
                            Members = new { onGrid = c.GetOngridCount(c.FleetAssignments.ToList()), max = c.GetFleetTypeMax() },
                            comms = new { c.CommChannel.LinkText, c.CommChannel.Url },
                            fc = (c.BossPilot != null) ? new { c.BossPilot.CharacterID, c.BossPilot.CharacterName } : null,
                            system = (c.System != null) ? new { c.System.Id, c.System.Name } : null
                        }).ToList());
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error getting fleets {0}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
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

            foreach(WaitingPilot pilot in waitingPilots)
            {
                pilot.RemovedByAccountId = User.AccountId();
            }

            _Db.SaveChanges();

            return Ok();
        }

        [HttpPost]
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
            } catch(Exception ex)
            {
                _Logger.LogWarning("{0} could not be added to the waitlist: {1}", pilot.CharacterName, ex.Message);
                return BadRequest($"Could not add {pilot.CharacterName} to the waitlist {ex.Message}");
            }
        }

        [HttpGet]
        [Route("/api/v1/user-settings")]
        [Produces("application/json")]
        public IActionResult UserSettings()
        {
            Account account = _Db.Accounts.Where(c => c.Id == User.AccountId()).Include(c => c.Pilots).Include(c => c.Fits).FirstOrDefault();
            // Account not found, user redirected to login
            if (account == null)
                return NotFound("We could not load your account, logout and back in.");

            List<FleetRole> roles = _Db.FleetRoles.Where(c => c.Avaliable).ToList();
            List<WaitingPilot> waitingPilots = _Db.WaitingPilots.Include(c => c.Pilot).Where(c => c.Pilot.AccountId == User.AccountId() && c.RemovedByAccountId == null).ToList();
            //?? ships[]
            return Ok(new UserSettingsResponse {
                account = account,
                waitingPilots = waitingPilots,
                avaliableFits = account.ActiveFits(),
                roles = roles,
                prefPilot = new PrefPilot { pilotId = Request.Cookies.PreferredPilotId(), Name = Request.Cookies.PreferredPilotName()}
            });
        }

        [HttpGet]
        [Route("/api/v1/fc-settings")]
        [Produces("application/json")]
        public IActionResult FcSettings()
        {
            if (!User.IsInRole("Commander") || !User.IsInRole("Leadership"))
                return Ok();

            try
            {
                var CommsChannels = _Db.CommChannels.ToList();
                List<string> FleetTypes = Enum.GetValues(typeof(FleetType)).Cast<FleetType>()
                                                                           .Select(v => v.ToString())
                                                                           .ToList();

                var PilotResults = _Db.Pilots.Where(a => a.AccountId == User.AccountId() && a.ESIValid).Select(c => new { c.CharacterID, c.CharacterName }).ToList();

                List<FleetBoss> pilotTuple = new List<FleetBoss>();

                foreach (var p in PilotResults)
                    pilotTuple.Add(
                        new FleetBoss{
                            Id = p.CharacterID,
                            Name = p.CharacterName
                        }
                    );

                
                return Ok(new FcSettingsResponse
                {
                    Comms = CommsChannels,
                    FleetTypes = FleetTypes,
                    Pilots = pilotTuple,
                    prefPilot = new PrefPilot { pilotId = Request.Cookies.PreferredPilotId(), Name = Request.Cookies.PreferredPilotName() }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Produces("application/json")]
        [Route("/waitlist/remove/{id}")]
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
    }

    struct FcSettingsResponse
    {
        public List<Models.CommChannel> Comms;
        public List<string> FleetTypes;
        public List<FleetBoss> Pilots;
        public PrefPilot prefPilot;
    }

    struct UserSettingsResponse
    {
        public Account account;
        public List<WaitingPilot> waitingPilots;
        public List<FleetRole> roles;
        public List<Fit> avaliableFits;
        public PrefPilot prefPilot;
    }

    struct FleetBoss
    {
        public int Id;
        public string Name;
    }

    struct PrefPilot
    {
        public int pilotId;
        public string Name;
    }
}