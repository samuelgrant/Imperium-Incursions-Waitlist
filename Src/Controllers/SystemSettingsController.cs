﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ESI.NET.Models;
using ESI.NET.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.EntityFrameworkCore;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Route("/admin/settings")]
    [Authorize(Roles ="Leadership,Dev")]
    public class SystemSettingsController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public SystemSettingsController(Data.WaitlistDataContext db, ILogger<SystemSettingsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [HttpGet]
        [Route("/admin/settings")]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/SystemSettings.cshtml");
        }

        [HttpGet("data")]
        [Produces("application/json")]
        public async Task<IActionResult> GetData()
        {
            var ships = await _Db.ShipTypes.Where(c => c.Queue != Queue.None).Select(s => new
            {
                s.Id,
                s.Name,
                s.Queue
            }).OrderBy(o => o.Name).ToListAsync();

            return Ok(new {
                hull = ships,
                queues = Enum.GetNames(typeof(Queue)).ToList()
            });
        }

        /// <summary>
        /// AUTO COMPLETE search for all ships
        /// </summary>
        [HttpGet("ships/search")]
        [Produces("application/json")]
        public async Task<IActionResult> SearchShips(string q)
        {
            return Ok(await _Db.ShipTypes.Where(c => c.Name.Contains(q)).Select(c => c.Name).ToListAsync());
        }

        /// <summary>
        /// Updates the Queue applied to a given ship
        /// </summary>
        [HttpPut("ships")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateShip(IFormCollection request)
        {
            ShipType ship = await _Db.ShipTypes.FindAsync(request._int("ship_id"));
            ship.Queue = (Queue)request._int("queue_id");

            await _Db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Adds a queue to a ship, if the ship is not in our database then we get it from ESI.
        /// </summary>
        [HttpPost("ships")]
        [Produces("application/json")]
        public async Task<IActionResult> NewShip(IFormCollection request)
        {
            string hullType = request._str("ship_name");
            int queue_id = request._int("queue_id");

            ShipType ship = await _Db.ShipTypes.Where(c => c.Name.ToLower() == hullType.ToLower()).FirstOrDefaultAsync();
            if(ship != null)
            {
                ship.Queue = (Queue)queue_id;
                await _Db.SaveChangesAsync();

                return Accepted();
            }

            SearchResults x = await EsiWrapper.Search(hullType, true, SearchCategory.InventoryType);
            if (x.InventoryTypes == null)
                return NotFound($"{hullType} could not be found. Is the name spelt correctly?");

            await _Db.AddAsync( new ShipType
            {
                Id = (int)x.InventoryTypes[0],
                Name = hullType,
                Queue = (Queue)queue_id
            });

            await _Db.SaveChangesAsync();

            return Ok();
        }
    }
}