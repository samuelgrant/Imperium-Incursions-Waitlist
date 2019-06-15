using System;
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
    [Route("/account-settings")]
    public class AccountSettingsController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private ILogger _Logger;

        public AccountSettingsController(Data.WaitlistDataContext db, ILogger<AccountSettingsController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        public IActionResult Index()
        {
            return View(viewName: "~/Views/AccountSettings.cshtml");
        }

        [HttpGet("data")]
        [Produces("application/json")]
        public async Task<IActionResult> Data()
        {
            Account account = await _Db.Accounts.FindAsync(User.AccountId());

            var fits = await _Db.Fits.Where(c => c.AccountId == User.AccountId() && !c.IsShipScan && c.DeletedAt == null)
            .Include(i => i.ShipType).Select(s => new
            {
                s.Id,
                s.Description,
                s.ShipTypeId,
                s.ShipType.Name
            }).ToListAsync();

            return Ok(new
            {
                account.JabberNotifications,
                Fits = fits
            });
        }

        [HttpPost("jabber")]
        [Produces("application/json")]
        public async Task<IActionResult> JabberNotificationSetting(IFormCollection request)
        {
            Account account = await _Db.Accounts.FindAsync(User.AccountId());
            account.JabberNotifications = (request._str("notificationsEnabled").ToLower() == "true")? true : false ;

            await _Db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("fit")]
        [Produces("application/json")]
        public async Task<IActionResult> Fit(IFormCollection request)
        {
            int currentFits = _Db.Fits.Where(c => c.AccountId == User.AccountId() && !c.IsShipScan && c.DeletedAt == null).Count();
            
            // Accounts are only allowed five fits at a time
            if (currentFits >= 5)// Does not take into account fits added by an FC through the fit scanner
                return BadRequest("You have reached your five ship limit. Please delete a ship before saving a new one");

            FitDna fitUrlObject;
            try
            {
                fitUrlObject = Util.ParseFitDna(request._str("fitUrl"));
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
                await _Db.AddAsync(newFit);
                await _Db.SaveChangesAsync();

                return Ok();
            }
            catch(Exception ex)
            {
                _Logger.LogError("{0} submitted an invalid fit URL {1}: {2}", User.AccountName(), request._str("fitUrl"), ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("fit/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> Fit(int id)
        {
            Fit fit = await _Db.Fits.Where(c => c.Id == id && c.AccountId == User.AccountId()).FirstOrDefaultAsync();

            if (fit == null)
                return NotFound("The fit was not found or you do not have access to it.");

            _Db.Remove(fit);
            await _Db.SaveChangesAsync();
            return Ok();
        }
    }
}