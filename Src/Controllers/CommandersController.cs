﻿using System;
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
            return View(viewName: "~/Views/Commanders.cshtml");
        }


        /// <summary>
        /// Returns a list of accounts that have one or more account roles. 
        /// </summary>
        [HttpGet]
        public IActionResult Active()
        {
            var fcs = _Db.Accounts
                .Include(a => a.Pilots)
                .Include(a => a.AccountRoles)
                    .ThenInclude(ar => ar.Role)
                .Where(a => a.AccountRoles.Count > 0)
                .OrderBy(s => s.Name);
               
            return Ok(fcs);
        }

        /// <summary>
        /// Returns a list of all available roles.
        /// </summary>
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _Db.Roles;
            return Ok(roles);
        }

        /// <summary>
        /// Adds a role to an account. 
        /// </summary>
        [HttpPost]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult AddRole(FormCollection request)
        {
            // Validate form inputs

            // Parse inputs as ints
            int.TryParse(request["accountId"], out int accountId);
            int.TryParse(request["roleId"], out int roleId);
            string accountName = request["accountName"];

            // Validate to ensure the required fields were returned.
            if (accountId == 0 && String.IsNullOrEmpty(accountName) || roleId == 0)
                return BadRequest("Invalid role or account ID/Name provided");            

            var account = _Db.Accounts.Where(a => a.Name == accountName || a.Id == accountId).SingleOrDefault();
            var role = _Db.Roles.Find(roleId);

            if (account == null)
                return NotFound("Account not found.");

            if (role == null)
                return NotFound("Role not found.");

            try
            {
                account.AccountRoles.Add(
                    new Models.AccountRole
                    {
                        Role = role,
                        Account = account
                    });

                _Db.SaveChanges();

                _Logger.LogInformation("{0} has added the {1} role to {2}", User.FindFirst("name").Value, role.Name, account.Name);
                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogWarning("AddRole: Error granting role to {0} : {1}", account.Name, ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Revokes a role from an account. 
        /// </summary>
        [HttpDelete]
        [Produces("application/json")]
        [Authorize(Roles = "Leadership")]
        public IActionResult RemoveRole(FormCollection request)
        {
            // Parse inputs as ints
            int.TryParse(request["accountId"], out int accountId);
            int.TryParse(request["roleId"], out int roleId);
            // Validate to ensure the required fields were returned.
            if (accountId == 0 || roleId == 0)
                return BadRequest("Invalid role or account ID provided");      
            

            var accountRole = _Db.AccountRoles
                                .Where(ar => ar.AccountId == accountId && ar.RoleId == roleId)
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
                return BadRequest();
            }
        }
    }
}