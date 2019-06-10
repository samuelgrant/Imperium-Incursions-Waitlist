using System;
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
        public IActionResult GetData()
        {
            var ships = _Db.ShipTypes.Where(c => c.Queue != Queue.None).Select(s => new
            {
                s.Id,
                s.Name,
                s.Queue
            }).OrderBy(o => o.Name).ToList();

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
        public IActionResult SearchShips(string q)
        {
            return Ok(_Db.ShipTypes.Where(c => c.Name.Contains(q)).Select(c => c.Name).ToList());
        }

        /// <summary>
        /// Updates the Queue applied to a given ship
        /// </summary>
        [HttpPut("ships")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateShip(IFormCollection request)
        {
            ShipType ship = await _Db.ShipTypes.FindAsync(int.Parse(request["ship_id"].ToString()));
            ship.Queue = (Queue)int.Parse(request["queue_id"].ToString());

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
            if (x.InventoryTypes == null)
                return NotFound($"{hullType} could not be found. Is the name spelt correctly?");

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