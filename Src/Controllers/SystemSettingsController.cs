using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Imperium_Incursions_Waitlist.Services;
using ESI.NET.Models;
using ESI.NET.Enumerations;

namespace Imperium_Incursions_Waitlist.Controllers
{
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

        /// <summary>
        /// Returns a list of all ships that are attached to a queue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Route("/admin/settings/ships")]
        public IActionResult GetShipQueues()
        {
            List<ShipType> ships = _Db.ShipTypes.Where(c => c.Queue != Queue.None).OrderBy(o => o.Name).ToList();

            return Ok(new {
                hull = ships,
                queues = Enum.GetNames(typeof(Queue)).ToList()
            });
        }

        /// <summary>
        /// AUTO COMPLETE search for all ships
        /// </summary>
        /// <param name="q">Query string to search</param>
        /// <returns>JSON output of ships matching the query</returns>
        [HttpGet]
        [Produces("application/json")]
        [Route("/admin/settings/ships/search")]
        public IActionResult SearchShips(string q)
        {
            return Ok(_Db.ShipTypes.Where(c => c.Name.Contains(q)).Select(c => c.Name).ToList());
        }

        /// <summary>
        /// Updates the Queue applied to a given ship
        /// </summary>
        [HttpPut]
        [Produces("application/json")]
        [Route("/admin/settings/ships")]
        public async Task<IActionResult> UpdateShip(IFormCollection request)
        {
            ShipType ship = await _Db.ShipTypes.FindAsync(int.Parse(request["ship_id"].ToString()));
            ship.Queue = (Queue)int.Parse(request["queue_id"].ToString());

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("/admin/settings/ships")]
        public async Task<IActionResult> NewShip(IFormCollection request)
        {
            string hullType = request["ship_name"].ToString();
            int queue_id = int.Parse(request["queue_id"].ToString());

            ShipType ship = _Db.ShipTypes.Where(c => c.Name.ToLower() == hullType.ToLower()).FirstOrDefault();
            if(ship != null)
            {
                ship.Queue = (Queue)queue_id;
                await _Db.SaveChangesAsync();

                return Accepted();
            }

            SearchResults x = await EsiWrapper.Search(hullType, true, SearchCategory.InventoryType);
            if (x == null)
                return NotFound($"{hullType} could not be found. The name must be spelt correctly.");

            _Db.Add( new ShipType
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