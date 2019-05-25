using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ESI.NET.Models.SSO;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;

namespace Imperium_Incursions_Waitlist.Controllers.Auth
{
    public class ApiController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

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
            Pilot pilot = _Db.Pilots.Find(int.Parse(Request.Cookies["prefPilot"].Split(':')[0]));
            await pilot.UpdateToken();
            

            EsiWrapper.ShowInfo((AuthorizedCharacterData)pilot, target_id);

            await _Db.SaveChangesAsync();

            return Ok();
        }
    }
}