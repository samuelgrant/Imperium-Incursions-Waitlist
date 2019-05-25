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
        [HttpGet]
        [Produces("application/json")]
        public IActionResult Pilots()
        {
            var userID = User.FindFirst("id").Value;
            var pilots = _Db.Pilots.Where(p => p.AccountId == int.Parse(userID)).OrderBy(s => s.CharacterName);

            if (pilots == null)
                return NotFound();

            return Ok(pilots);
        }

        /// <summary>
        /// Sets a prefPilot cookie indicating the users preferred pilot
        /// </summary>
        [HttpPost]
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
 