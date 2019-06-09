using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Route("/admin/commanders")]
    [Authorize(Roles = "Leadership, Commander")]
    public class CommandersController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

        public CommandersController(Data.WaitlistDataContext db, ILogger<CommandersController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/UserManagement.cshtml");
        }


        /// <summary>
        /// Returns a list of accounts that have one or more account roles. 
        /// </summary>
        [HttpGet("data")]
        public IActionResult Data()
        {
            var fcs = _Db.Accounts.Include(i => i.Pilots).Include(i => i.AccountRoles).ThenInclude(i => i.Role)
                .Where(c => c.AccountRoles.Count > 0).Select( s => new {
                s.Id,
                s.Name,
                s.LastLogin,
                roles = s.AccountRoles.Select(s1 => new { id = s1.Role.Id, name = s1.Role.Name}),
                pilots = s.Pilots.Select(s2 => new {
                    id = s2.CharacterID,
                    name = s2.CharacterName,
                    corporation = new { id = s2.Corporation.Id, name = s2.Corporation.Name },
                    alliance = new { id = s2.Corporation.Alliance.Id, name = s2.Corporation.Alliance.Name},
                })
            }).OrderBy(o => o.Name).ToList();

            var roles = _Db.Roles;
            return Ok(new {
                fcs,
                roles
            });
        }

        /// <summary>
        /// Adds a role to an account. 
        /// </summary>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Index(IFormCollection request)
        {
            // Parse inputs as ints
            int.TryParse(request["account_id"], out int accountId);
            int.TryParse(request["role_id"], out int roleId);
            string accountName = request["account_name"];

            // Validate to ensure the required fields were returned.
            if (accountId == 0 && String.IsNullOrEmpty(accountName) || roleId == 0)
                return BadRequest("Invalid role or account ID/Name provided");

            var account = _Db.Accounts.Where(a => a.Name == accountName || a.Id == accountId).SingleOrDefault();
            
            var role = _Db.Roles.Find(roleId);

            // User account does not exist
            if (account == null)
                return NotFound("Account not found.");

            // Stops a user from changing their own role
            if (account.Id == User.AccountId())
                return Unauthorized("You are not allowed to add your own groups");

            // Role doesn't exist
            if (role == null)
                return NotFound("Role not found.");

            try
            {

                _Db.AccountRoles.Add(new Models.AccountRole
                {
                    AccountId = account.Id,
                    RoleId = role.Id,
                });

                _Db.SaveChanges();

                _Logger.LogInformation("{0} has added the {1} role to {2}", User.FindFirst("name").Value, role.Name, account.Name);
                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogWarning("AddRole: Error granting role to {0} : {1}", account.Name, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Revokes a role from an account. 
        /// </summary>
        [HttpDelete("revoke")]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult Revoke(IFormCollection request)
        {
            // Parse inputs as ints
            int.TryParse(request["accountId"], out int accountId);
            int.TryParse(request["roleId"], out int roleId);
            // Validate to ensure the required fields were returned.
            if (accountId == 0 || roleId == 0)
                return BadRequest("Invalid role or account ID provided");

            if (accountId == User.AccountId())
                return Unauthorized("You are not allowed to remove your own groups");
            

            var accountRole = _Db.AccountRoles
                                .Where(ar => ar.AccountId == accountId && ar.RoleId == roleId)
                                .Include(ar => ar.Account)
                                .Include(ar => ar.Role)
                                .SingleOrDefault();

            if (accountRole == null)
                return NotFound();

            try
            {
                _Db.Remove(accountRole);
                _Db.SaveChanges();

                _Logger.LogInformation("{0} role revoked from {1}", accountRole.Role.Name, accountRole.Account.Name);

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogWarning("RemoveRole: Error revoking role from {0}: {1}", accountRole.Account.Name, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}