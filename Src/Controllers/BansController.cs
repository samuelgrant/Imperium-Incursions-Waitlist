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
    [Route("/admin/bans")]
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
        /// Returns a list of active bans. Active bans are bans that have not expired, or have no expiry date.
        /// </summary>
        [HttpGet("active")]
        [Produces("application/json")]
        public IActionResult Active()
        {
            var bans = _Db.Bans.Where(c => c.ExpiresAt > DateTime.UtcNow || c.ExpiresAt == null)
                .Include(i => i.BannedAccount).ThenInclude(i => i.Pilots).ThenInclude(i => i.Corporation).ThenInclude(i => i.Alliance).Select(s => new
                {
                    s.Id,
                    banAdmin = new {
                        id = s.UpdatingAdmin == null ? s.CreatorAdmin.Id : s.UpdatingAdmin.Id,
                        name = s.UpdatingAdmin == null ? s.CreatorAdmin.Name : s.UpdatingAdmin.Name },
                    bannedAccount = new {
                        id = s.BannedAccount.Id,
                        name = s.BannedAccount.Name,
                        pilots = s.BannedAccount.Pilots.Select(s1 => new {
                            id = s1.CharacterID,
                            name = s1.CharacterName
                        })
                    },
                    s.ExpiresAt,
                    s.Reason,
                    s.CreatedAt
                }).OrderBy(o => o.bannedAccount.name).ToList();
            return Ok(bans);
        }

        /// <summary>
        /// Issues a ban against a user account
        /// </summary>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Index(IFormCollection request)
        {
            if (request["reason"] == "" || request["name"] == "")
                return BadRequest();

            int AdminId = User.AccountId();
            var BannedAccount = _Db.Accounts.FirstOrDefault(c => c.Name == request["name"]);

            if (BannedAccount == null)
                return NotFound(string.Format("The account {0} was not found", request["name"]));

            if (AdminId == BannedAccount.Id)
                return Forbid("You cannot ban yourself");
           
            try
            {
                Ban ban = new Ban()
                {
                    AdminId = AdminId,
                    BannedAccountId = BannedAccount.Id,
                    Reason = request["reason"],
                    //Expires at is disabled until I can spend enough time to use a proper Jquery date picker
                    ExpiresAt = null,//Ban.BanExpiryDate(request["expires_at"]),


                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _Logger.LogInformation("{0} is issuing a ban against {1}", User.FindFirst("name").Value, request["name"]);

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
        [HttpPut("{id}")]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Update(IFormCollection request, int id)
        {
            var currentBan = _Db.Bans.Include(c => c.BannedAccount).FirstOrDefault(c => c.Id == id);
            
            // If no ban was found return 404
            if (currentBan == null)
                return NotFound();

            try
            {
                currentBan.Reason = request["reason"];
                //Expires at is disabled until I can spend enough time to use a proper Jquery date picker
                currentBan.ExpiresAt = null;//Ban.BanExpiryDate(request["expires_at"]),
                currentBan.UpdatedByAdminId = User.AccountId();
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
        [HttpDelete("{id}")]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Revoke(int id)
        {
            var currentBan = _Db.Bans.Include(c => c.BannedAccount).SingleOrDefault(c => c.Id == id);
            string baneeName = currentBan?.BannedAccount?.Name;

            if (currentBan == null)
                return NotFound();
            
            try
            {
                currentBan.ExpiresAt = DateTime.UtcNow;

                _Db.SaveChanges();

                _Logger.LogInformation("{0} has revoked ban against {1}", User.FindFirst("name").Value, baneeName);
            }
            catch (Exception ex)
            {
                _Logger.LogInformation("Error revoking ban against {0}: {1}", baneeName, ex.Message);
                return BadRequest("Failed to revoke ban.");
            }

            return Ok();
        }
    }
}