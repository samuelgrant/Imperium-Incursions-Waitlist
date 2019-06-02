using System;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using DotNetEnv;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using GiceSSO;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class GiceController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private IPAddress _RequestorIP;
        private ILogger _Logger;
        private ClientConfig _GiceConfig;

        public GiceController(Data.WaitlistDataContext db, IHttpContextAccessor clientAccessor, ILogger<GiceController> logger)
        {
            _Db = db;
            _RequestorIP = clientAccessor.HttpContext.Connection.RemoteIpAddress;
            _Logger = logger;

            // Setup a Gice SSO Client
            Env.Load();
            _GiceConfig = new ClientConfig
            {
                ClientId = Env.GetString("gice_clientID"),
                SecretKey = Env.GetString("gice_clientSecret"),
                UserAgent = "Imperium Incursions. Contact: samuel_the_terrible"
            };
        }

        /// <summary>
        /// Initiates GICE SSO workflow
        /// </summary>
        [AllowAnonymous]
        public ActionResult Go()
        {
            _GiceConfig.CallbackUrl = Url.Action("callback", "gice", null, protocol: "https").ToLower();

            //Create a state and save to session
            string newState = Util.RandomString(6);
            HttpContext.Session.SetString("state", newState);

            GiceClientv2 gice = new GiceClientv2(_GiceConfig);
            
            // Request Authentication from GICE.
            return Redirect(gice.GenerateRequestUrl(newState));
        }

        /// <summary>
        /// Handles the GICE SSO callback.
        /// </summary>
        [AllowAnonymous]
        [ActionName("callback")]
        public async Task<IActionResult> Callback(string code, string state)
        {
            // Verify a code and state query parameter was returned.
            if (code == null || state == null)
            {
                _Logger.LogWarning("GICE Callback Error: One or more of the query parameters are missing. State: {0}. Code: {1}", state, code);
                return StatusCode(452);
            }

            // Verify the state to protect against CSRF attacks.
            if (HttpContext.Session.GetString("state") != state)
            {
                _Logger.LogWarning("GICE Callback Error: Invalid state returned.");
                HttpContext.Session.Remove("state");
                return StatusCode(452);
            }

            // Clear the state session
            HttpContext.Session.Remove("state");

            // Set the callback URL
            _GiceConfig.CallbackUrl = Url.Action("Callback", "Gice", null, "https").ToLower();

            GiceClientv2 gice = new GiceClientv2(_GiceConfig);
            Dictionary<string, string> ClaimsDict = gice.VerifyCode(code);
            

            if(ClaimsDict.Count == 0)
            {
                _Logger.LogWarning("GICE Callback Error: The JwtVerify method failed.");
                return StatusCode(452);
            }

            // Do we have an account for this GSF User?
            int gice_id = int.Parse(ClaimsDict["sub"]);
            var waitlist_account = await _Db.Accounts.FindAsync(gice_id);
            if(waitlist_account != null)
            {
                // Update and save user account
                waitlist_account.Name = ClaimsDict["name"];
                waitlist_account.LastLogin = DateTime.UtcNow;
                waitlist_account.LastLoginIP = _RequestorIP.MapToIPv4().ToString();

                _Db.Update(waitlist_account);
                await _Db.SaveChangesAsync();
            }
            else
            {
                // User doesn't exist, create a new account
                waitlist_account = new Account()
                {
                    Id = gice_id,
                    Name = ClaimsDict["name"],
                    RegisteredAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    LastLoginIP = _RequestorIP.MapToIPv4().ToString()
                };

                _Db.Add(waitlist_account);
                await _Db.SaveChangesAsync();
            }

            // Attempt to log the user in
            await LoginUserUsingId(waitlist_account.Id);

            _Logger.LogDebug("{0} has logged in.", ClaimsDict["name"]);

            return Redirect("~/pilot-select");            
        }

        [Authorize]
        public async Task<string> Logout()
        {
            _Logger.LogDebug("{0} has logged out.", User.FindFirst("name").Value);

            // Log the user out of our application
            await HttpContext.SignOutAsync();

            // Delete special cookies
            Response.Cookies.Delete("prefPilot");

            // Redirect to goonswarm for GICE logout.
            //return Redirect("https://esi.goonfleet.com/oauth/revoke");

            return "You were logged out!";
        }

        // DO NOT KEEP THIS METHOD IN PRODUCTION!!!!!!!!!!!!!!!	
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithId(int id)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await LoginUserUsingId(id);
            } 

            return Redirect("~/");
        }

        /// <summary>
        /// Logs a user in using a specific ID
        /// </summary>
        /// <param name="id">Account ID - GICE API returns this as the "sub"</param>
        /// <returns>True for a successful login, false for a failure</returns>
        public async Task<bool> LoginUserUsingId(int id = -1)
        {
            // If a user ID was not supplied, fail the login process.
            if (id == -1) return false;

            // Eager load the account and the related roles data
            var account = await _Db.Accounts
                                .Include(a => a.AccountRoles)
                                    .ThenInclude(ar => ar.Role)
                                .SingleOrDefaultAsync(ar => ar.Id == id);

            // Fail if the account is not in the database
            if (account == null) return false;                        

            var claims = new List<Claim>
            {
                new Claim("id", account.Id.ToString()),
                new Claim("name", account.Name)
            };

            // Add a claim for each role type associated to the account
            foreach (var role in account.AccountRoles.Select(ar => ar.Role))            
                claims.Add(new Claim(ClaimTypes.Role, role.Name));            

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // Refreshing the authentication session should be allowed.
                AllowRefresh = true,

                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),

                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.
                IsPersistent = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return true;
        }
    }
}