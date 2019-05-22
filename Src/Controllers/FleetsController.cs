using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize(Roles = "Commander")]
    public class FleetsController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

        public FleetsController(Data.WaitlistDataContext db, ILogger<FleetsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [Route("/fleets/{id}")]
        public IActionResult Index(int id)
        {
            ViewData["fleetId"] = id;
            return View(viewName: "~/Views/FleetManagement.cshtml");
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
                BossId = bossId,
                CommChannelId = comms.Id,
                IsPublic = false,
                Type = fleetType
            };

            _Db.Fleets.Add(newFleet);
            _Db.SaveChanges();

            // Redirect to page!
            return Ok(newFleet.Id);
        }
    }
}