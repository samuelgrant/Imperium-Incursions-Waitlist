using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using DotNetEnv;
using Newtonsoft.Json;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class GiceController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private IPAddress _RequestorIP;
        private ILogger _Logger;

        public GiceController(Data.WaitlistDataContext db, IHttpContextAccessor clientAccessor, ILogger<GiceController> logger)
        {
            _Db = db;
            _RequestorIP = clientAccessor.HttpContext.Connection.RemoteIpAddress;
            _Logger = logger;
        }

        /// <summary>
        /// Initiates GICE SSO workflow
        /// </summary>
        [AllowAnonymous]
        public ActionResult Go()
        {
            Env.Load();

            //Get Client ID
            string clientID = Env.GetString("gice_clientID");


            //Callback
            string redirectUri = Url.Action("callback", "gice", null, protocol: "https").ToLower();


            //Create a state and save to session
            string newState = Util.RandomString(6);
            HttpContext.Session.SetString("state", newState);
            
            // Request Authentication from GICE.
            return Redirect(string.Format(
                "https://esi.goonfleet.com/oauth/authorize?client_id={0}&redirect_uri={1}&response_type={2}&state={3}",
                clientID,
                redirectUri,
                "code",
                newState
            ));
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
                _Logger.LogWarningFormat("GICE Callback Error: One or more of the query parameters are missing. State: {0}. Code: {1}", state, code);
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

            // Get the authorization code
            var postData = new NameValueCollection();
            var client = new WebClient();

            postData["grant_type"] = "authorization_code";
            postData["code"] = code;
            postData["redirect_uri"] = Url.Action("Callback", "Gice", null, "https").ToLower();
            postData["client_id"] = Env.GetString("gice_clientID");
            postData["client_secret"] = Env.GetString("gice_clientSecret");

            var response = client.UploadValues("https://esi.goonfleet.com/oauth/token", postData);
            // Convert a JSON String into a usable object
            var data = Encoding.Default.GetString(response);
            var model = JsonConvert.DeserializeObject<dynamic>(data);
            string access_token = model["access_token"].Value;

            // Vlidate and Decode the JWT token
            Dictionary<string, string> user = await Util.JwtVerify("https://esi.goonfleet.com/", "https://esi.goonfleet.com/", access_token);
            if(user.Count == 0)
            {
                _Logger.LogWarningFormat("GICE Callback Error: The JwtVerify method failed.");
                return StatusCode(452);
            }

            // Do we have an account for this GSF User?
            int gice_id = int.Parse(user["sub"]);
            var waitlist_account = await _Db.Accounts.FindAsync(gice_id);
            if(waitlist_account != null)
            {
                // Update and save user account
                waitlist_account.Name = user["name"];
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
                    Name = user["name"],
                    RegisteredAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    LastLoginIP = _RequestorIP.MapToIPv4().ToString()
                };

                _Db.Add(waitlist_account);
                await _Db.SaveChangesAsync();
            }

            // Attempt to log the user in
            await LoginUserUsingId(waitlist_account.Id);
            _Logger.LogDebugFormat("{0} has logged in.", user["name"]);

            return Redirect("~/pilot-select");            
        }

        [Authorize]
        public async Task<string> Logout()
        {
            _Logger.LogDebugFormat("{0} has logged out.", User.FindFirst("name").Value);

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

            // Eager load the accounts and the related roles data
            // (can we do this just for the one account?)
            var accounts = await _Db.Accounts
                                .Include(a => a.AccountRoles)
                                    .ThenInclude(ar => ar.Role)
                                .ToListAsync();

            // Look up the database for the account.
            // If no account is found fail the login and return.
            var account = accounts.Find(a => a.Id == id);
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