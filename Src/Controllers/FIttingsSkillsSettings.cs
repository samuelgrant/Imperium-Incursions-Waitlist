using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class AccountSettingsController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

        public AccountSettingsController(Data.WaitlistDataContext db, ILogger<AccountSettingsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [Route("/account-settings")]
        public IActionResult Index()
        {
            return View(viewName: "~/Views/AccountSettings.cshtml");
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("/account-settings/data")]          
        public async Task<IActionResult> Data()
        {
            Account account = await _Db.Accounts.FindAsync(User.AccountId());
            AccountSettings settingsResult = new AccountSettings {
                AllowsJabberNotifications = account.JabberNotifications,
            };

            return Ok(settingsResult);
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("/account-settings/notifications")]
        public async Task<IActionResult> JabberNotificationSetting(IFormCollection request)
        {
            Account account = await _Db.Accounts.FindAsync(User.AccountId());
            account.JabberNotifications = (request["notificationsEnabled"].ToString().ToLower() == "true")? true : false ;

            await _Db.SaveChangesAsync();

            return Ok();
        }

        public struct AccountSettings
        {
            public bool AllowsJabberNotifications;
        }
    }
}