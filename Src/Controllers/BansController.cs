using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Active()
        {
            var bans = await _Db.Bans.Where(c => c.ExpiresAt > DateTime.UtcNow || c.ExpiresAt == null)
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
                }).OrderBy(o => o.bannedAccount.name).ToListAsync();

            return Ok(new {
                bans,
                admin = User.IsInRole("Leadership")
            });
        }

        /// <summary>
        /// Issues a ban against a user account
        /// </summary>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public async Task<IActionResult> Index(IFormCollection request)
        {
            if (request._str("banReason") == "" || request._str("accountName") == "")
                return BadRequest();

            int AdminId = User.AccountId();
            var BannedAccount = await _Db.Accounts.FirstOrDefaultAsync(c => c.Name == request._str("accountName"));

            if (BannedAccount == null)
                return NotFound(string.Format("The account {0} was not found", request._str("accountName")));

            if (AdminId == BannedAccount.Id)
                return Forbid("You cannot ban yourself");
           
            try
            {
                Ban ban = new Ban()
                {
                    AdminId = AdminId,
                    BannedAccountId = BannedAccount.Id,
                    Reason = request._str("banReason"),
                    //Expires at is disabled until I can spend enough time to use a proper Jquery date picker
                    ExpiresAt = null,//Ban.BanExpiryDate(request["expires_at"]),


                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _Logger.LogInformation("{0} is issuing a ban against {1}", User.AccountName(), request._str("accountName"));

                await _Db.Bans.AddAsync(ban);
                await _Db.SaveChangesAsync();
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
        public async Task<IActionResult> Update(IFormCollection request, int id)
        {
            var currentBan = await _Db.Bans.Include(c => c.BannedAccount).FirstOrDefaultAsync(c => c.Id == id);
            
            // If no ban was found return 404
            if (currentBan == null)
                return NotFound();

            try
            {
                currentBan.Reason = request._str("banReason");
                //Expires at is disabled until I can spend enough time to use a proper Jquery date picker
                currentBan.ExpiresAt = null;//Ban.BanExpiryDate(request["expires_at"]),
                currentBan.UpdatedByAdminId = User.AccountId();
                currentBan.UpdatedAt = DateTime.UtcNow;

                _Logger.LogInformation("{0} is updating the ban against {1}", User.AccountName(), currentBan.BannedAccount.Name);
                await _Db.SaveChangesAsync();
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
        public async Task<IActionResult> Revoke(int id)
        {
            var currentBan = await _Db.Bans.Include(c => c.BannedAccount).SingleOrDefaultAsync(c => c.Id == id);
            string baneeName = currentBan?.BannedAccount?.Name;

            if (currentBan == null)
                return NotFound();
            
            try
            {
                currentBan.ExpiresAt = DateTime.UtcNow;

                await _Db.SaveChangesAsync();
                _Logger.LogInformation("{0} has revoked ban against {1}", User.AccountName(), baneeName);
                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogInformation("Error revoking ban against {0}: {1}", baneeName, ex.Message);
                return BadRequest("Failed to revoke ban.");
            }
        }
    }
}