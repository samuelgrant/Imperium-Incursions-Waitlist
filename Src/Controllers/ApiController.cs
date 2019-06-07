using System.Linq;
using ESI.NET.Models.SSO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;

namespace Imperium_Incursions_Waitlist.Controllers.Auth
{
    public class ApiController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public ApiController(Data.WaitlistDataContext db, ILogger<ApiController> logger)
        {
            _Db = db;
            _Logger = logger;
        }


        [HttpPost]
        [Route("/api/esi-ui/show-info")]
        public async Task<IActionResult> ShowInfo(IFormCollection request)
        {
            int target_id = int.Parse(request["target_id"].ToString());
            Pilot pilot = _Db.Pilots.Find(Request.Cookies.PreferredPilotId());
            await pilot.UpdateToken();

            EsiWrapper.ShowInfo((AuthorizedCharacterData)pilot, target_id);

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("/api/esi-ui/destination")]
        public async Task<IActionResult> SetDestination(IFormCollection request)
        {
            int target_id = int.Parse(request["target_id"].ToString());
            Pilot pilot = _Db.Pilots.Find(Request.Cookies.PreferredPilotId());
            await pilot.UpdateToken();

            EsiWrapper.SetDestination((AuthorizedCharacterData)pilot, target_id);

            await _Db.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        [Produces("application/json")]
        [Route("/api/v1/waitlist/pilots")]
        [Authorize(Roles = "Commander,Leadership,Dev")]
        public async Task<IActionResult> GetWaitingPilots()
        {
            var waitlist =  _Db.WaitingPilots
                .Where(c => c.RemovedByAccountId == null)
                .Include(a => a.SelectedRoles)
                .Include(c => c.SelectedFits)
                .Select(c => new {
                    c.Id,
                    c.Pilot,
                    account = new{c.Pilot.AccountId, c.Pilot.Account.Name },
                    c.IsOffline,
                    c.NewPilot,
                    c.SelectedFits,
                    //?.Fit
                    roles = c.SelectedRoles,//?.FleetRole
                    system = (c.System != null) ? new { c.System.Id, c.System.Name } : null,
                    altInFleet = "",
                    c.WaitingFor,
                }).ToList();


            return Ok(waitlist);
        }
    }
}