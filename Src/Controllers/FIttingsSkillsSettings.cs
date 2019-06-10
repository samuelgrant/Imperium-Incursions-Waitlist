using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            List<Fit> fits = await _Db.Fits.Where(c => c.AccountId == User.AccountId() && !c.IsShipScan && c.DeletedAt == null)
                                            .Include(ci => ci.ShipType).ToListAsync();

            AccountSettings settingsResult = new AccountSettings {
                AllowsJabberNotifications = account.JabberNotifications,
                Fits = fits
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

        [HttpPost]
        [Produces("application/json")]
        [Route("/account-settings/fit")]
        public async Task<IActionResult> Fit(IFormCollection request)
        {
            int currentFits = _Db.Fits.Where(c => c.AccountId == User.AccountId() && !c.IsShipScan && c.DeletedAt == null).Count();
            
            // Accounts are only allowed five fits at a time
            if (currentFits >= 5)// Does not take into account fits added by an FC through the fit scanner
                return BadRequest("You have reached your five ship limit. Please delete a ship before saving a new one");

            FitDna fitUrlObject;
            try
            {
                fitUrlObject = Util.ParseFitDna(request["fitUrl"].ToString());
                Fit newFit = new Fit
                {
                    AccountId = User.AccountId(),
                    ShipTypeId = fitUrlObject.ship_typeId,
                    FittingDNA = fitUrlObject.dna,
                    Description = fitUrlObject.description,
                    IsShipScan = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Save new fit
                _Db.Add(newFit);
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch(Exception ex)
            {
                _Logger.LogError("{0} submitted an invalid fit URL {1}: {2}", User.FindFirst("Name").ToString(), request["fitUrl"].ToString(), ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Produces("application/json")]
        [Route("/account-settings/fit/{id}")]
        public async Task<IActionResult> Fit(int id)
        {
            Fit fit = await _Db.Fits.Where(c => c.Id == id && c.AccountId == User.AccountId()).FirstOrDefaultAsync();

            if (fit == null)
                return NotFound("The fit was not found or you do not have access to it.");

            _Db.Remove(fit);
            await _Db.SaveChangesAsync();
            return Ok();
        }

        public struct AccountSettings
        {
            public bool AllowsJabberNotifications;
            public List<Fit> Fits;
        }
    }
}