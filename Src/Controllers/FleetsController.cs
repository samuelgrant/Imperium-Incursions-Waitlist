﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if(fleet == null)
                // Fleet not found
                return NotFound("Fleet not found.");

            ViewData["fleetId"] = id;
            return View(viewName: "~/Views/FleetManagement.cshtml");
        }

        [HttpGet]
        [Route("/fleets/{id}/data")]
        [Produces("application/json")]
        public IActionResult Data(int id)
        {

            var Fleet = _Db.Fleets.Include(i => i.BossPilot)
                                  .Include(i => i.CommChannel).Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (Fleet == null)
                return NotFound($"Fleet {id} not found.");


            return Ok(Fleet);
        }

        /// <summary>
        /// Sets the fleet comms for the fleet
        /// </summary>
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/fleets/{id}/comms")]
        [Produces("application/json")]
        public IActionResult Comms(IFormCollection request, int id)
        {
            int commsId = int.Parse(request["commsId"].ToString());

            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Cannot write changes to a fleet that is not open.
                return NotFound("Fleet not found.");

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
        [HttpPut]
        [Route("/fleets/{id}/status")]
        [Produces("application/json")]
        public IActionResult Status(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Cannot write changes to a fleet that is not open.
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

        [HttpPut]
        [Route("/fleets/{id}/type")]
        [Produces("application/json")]
        public IActionResult Type(IFormCollection request, int id)
        {
            var fleet = _Db.Fleets.Where(c => c.Id == id && c.ClosedAt == null).FirstOrDefault();
            if (fleet == null)
                // Cannot write changes to a fleet that is not open.
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
            fleet.ClosedAt = DateTime.UtcNow;

            _Db.SaveChanges();

            return Ok();
        }
    }
}