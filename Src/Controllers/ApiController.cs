using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ESI.NET.Models.SSO;
using Imperium_Incursions_Waitlist.Models;

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
        public IActionResult ShowInfo(IFormCollection request)
        {
            int target_id = int.Parse(request["target_id"].ToString());
            Pilot pilot = _Db.Pilots.Find(int.Parse(User.FindFirst("Id").Value));

            // Get Authorised character data --- Probably a pilot method that returns an authorized character object and updates the refresh token for nextime

            // Call the ESI wrapper and supplies the authorised character data and target ID to make the call

            return Ok();
        }
    }
}