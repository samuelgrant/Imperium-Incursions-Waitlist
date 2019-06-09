using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Imperium_Incursions_Waitlist.Controllers.Auth
{
    [Authorize]
    [Route("/pilot-select")]
    public class PilotSelectController : Controller
    {
        private Data.WaitlistDataContext _Db;

        public PilotSelectController(Data.WaitlistDataContext db)
        {
            _Db = db;
        }

        /// <summary>
        /// Returns the Pilot Select View
        /// </summary>
        public IActionResult Index()
        {
            return View(viewName: "~/Views/PilotSelect.cshtml");
        }

        /// <summary>
        /// Returns a list of all pilots linked to the account.
        /// </summary>
        [HttpGet("pilots")]
        [Produces("application/json")]
        public IActionResult Pilots()
        {
            //var pilots = _Db.Pilots.Where(p => p.AccountId == User.AccountId()).OrderBy(s => s.CharacterName);

            var pilots = _Db.Pilots.Where(c => c.AccountId == User.AccountId()).Select(s => new
            {
                id = s.CharacterID,
                name = s.CharacterName,
                EsiValid = s.ESIValid
            }).OrderBy(o => o.name).ToList();

            if (pilots == null)
                return NotFound();

            return Ok(pilots);
        }

        /// <summary>
        /// Sets a prefPilot cookie indicating the users preferred pilot
        /// </summary>
        [HttpPost("pilots/{id}")]
        public async Task<IActionResult> Pilots(int id = 0)
        {
            var pilot = await _Db.Pilots.FindAsync(id);

            if (pilot == null)
                return BadRequest();

            if (!pilot.BelongsToAccount(User.AccountId()))
                return Unauthorized();
            
            CookieOptions options = new CookieOptions
            {
                IsEssential = true   
            };

            Response.Cookies.Append("prefPilot", pilot.CharacterID + ":" + pilot.CharacterName, options);

            return Ok();
        }
    }
}
 