
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class WaitlistController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

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
                        .Select(c => new {
                            c.Id,
                            c.Type,
                            c.MemberCount,
                            c.SystemId,
                            comms = new { c.CommChannel.LinkText, c.CommChannel.Url },
                            fc = new { c.BossPilot.CharacterID, c.BossPilot.CharacterName }
                        }).ToList());
                }
                else
                {
                    return Ok(_Db.Fleets.Where(c => c.ClosedAt == null && c.IsPublic)
                        .Select(c => new {
                            c.Id,
                            c.Type,
                            c.MemberCount,
                            c.SystemId,
                            comms = new { c.CommChannel.LinkText, c.CommChannel.Url },
                            fc = new { c.BossPilot.CharacterID, c.BossPilot.CharacterName }
                        }).ToList());
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error getting fleets {0}", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/api/v1/fc-settings")]
        [Produces("application/json")]
        public IActionResult FcSettings()
        {
            if (!User.IsInRole("Commander"))
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
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    struct FcSettingsResponse
    {
        public List<Models.CommChannel> Comms;
        public List<string> FleetTypes;
        public List<FleetBoss> Pilots;
    }

    struct FleetBoss
    {
        public int Id;
        public string Name;
    }

}