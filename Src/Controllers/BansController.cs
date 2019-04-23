using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize(Roles = "Leadership, Commander")]
    public class BansController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

        public BansController(Data.WaitlistDataContext db, ILogger<BansController> logger)
        {
            _Db = db;
            _Logger = logger;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/Bans.cshtml");
        }

        /// <summary>
        /// Returns a list of active bans. 
        /// Active bans are any ban that has not expired, or has no expiry date.
        /// </summary>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult Active()
        {
            var bans = _Db.Bans.Where(b => b.ExpiresAt > DateTime.UtcNow || b.ExpiresAt == null)
                .OrderBy(c => c.BannedAccount.Name)
                .Include(c => c.BannedAccount)
                    .ThenInclude(ba => ba.Pilots)
                .Include(c => c.CreatorAdmin);
            return Ok(bans);
        }

        /// <summary>
        /// Issues a ban against a user account
        /// </summary>
        /// <returns>HttpStatusCode</returns>
        /// <see cref="StatusCodes"/>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        [ActionName(null)]
        public IActionResult Bans(FormCollection request)
        {
            if (request["reason"] != "" || request["name"] != "")
                return BadRequest();

            int AdminId = int.Parse(User.FindFirst("id").Value);
            int BannedAccountId = _Db.Accounts.First(c => c.Name == request["name"]).Id;

            if (AdminId == BannedAccountId)
                return Forbid("You cannot ban yourself");
           
            try
            {
                Ban ban = new Ban()
                {
                    AdminId = AdminId,
                    BannedAccountId = BannedAccountId,
                    Reason = request["reason"],
                    ExpiresAt = Ban.BanExpiryDate(request["expires_at"]),

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _Logger.LogInformation("{0} is issuing a ban against {1}", User.FindFirst("name").Value, ban.BannedAccount.Name);

                _Db.Bans.Add(ban);
                _Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _Logger.LogWarning("Error issuing a new ban: {1}", ex.Message);
                return BadRequest("Failed to issue ban.");
            }

            
            return Ok();
        }

        /// <summary>
        /// Updates a ban against a user account. Enables leadership to update the reason or expiry
        /// </summary>
        /// <param name="banId">URL Paramater: ID of ban to be updated</param>
        /// <returns>HttpStatusCode</returns>
        /// <see cref="StatusCodes"/>
        [HttpPut]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Update(FormCollection request, int banId = 0)
        {
            // If no ban ID was supplied return 400
            if (banId == 0)
                return BadRequest();

            var currentBan = _Db.Bans.Find(banId);
            
            // If no ban was found return 404
            if (currentBan == null)
                return NotFound();

            try
            {
                currentBan.Reason = request["reason"];
                currentBan.ExpiresAt = Ban.BanExpiryDate(request["expires_at"]);
                currentBan.UpdatedByAdminId = int.Parse(User.FindFirst("id").Value);
                currentBan.UpdatedAt = DateTime.UtcNow;

                _Logger.LogInformation("{0} is updating the ban against {1}", User.FindFirst("name").Value, currentBan.BannedAccount.Name);
                _Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _Logger.LogWarning("Error updating ban (Ban Id: {0}): {1}", currentBan.Id, ex.Message);
                return BadRequest("Failed to update ban.");
            }
            

            return Ok();
        }

        /// <summary>
        /// Soft deletes a ban against a specific user account.
        /// </summary>
        /// <param name="id">URL Paramater: ID of ban to be revoked</param>
        /// <returns>HttpStatusCode</returns>
        /// <see cref="StatusCodes"/>
        [HttpDelete]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Revoke(int id = 0)
        {
            // Fail if no ban id is provided
            if (id == 0)
                return BadRequest();
            

            var currentBan = _Db.Bans.Find(id);

            if (currentBan == null)
                return NotFound();
            
            try
            {
                currentBan.ExpiresAt = DateTime.UtcNow;

                _Db.SaveChanges();

                _Logger.LogInformation("{0} has revoked ban against {1}", User.FindFirst("name").Value, currentBan.BannedAccount.Name);
            }
            catch (Exception ex)
            {
                _Logger.LogInformation("Error revoking ban against {0}: {1}", currentBan.BannedAccount.Name, ex.Message);
                return BadRequest("Failed to revoke ban.");
            }

            return Ok();
        }
    }
}