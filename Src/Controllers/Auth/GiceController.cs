using DotNetEnv;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Imperium_Incursions_Waitlist.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class GiceController : Controller
    {
        private Data.WaitlistDataContext _Db;
        private IPAddress _RequestorIP;

        public GiceController(Data.WaitlistDataContext db, IHttpContextAccessor clientAccessor)
        {
            _Db = db;
            _RequestorIP = clientAccessor.HttpContext.Connection.RemoteIpAddress;
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
                Log.Error(string.Format("GiceController@Callback - Callback Error one or more query paramaters were missing\nState: {0}\nCode: {1}", state, code));
                return StatusCode(452);
            }

            // Verify the state to protect against CSRF attacks.
            if (HttpContext.Session.GetString("state") != state)
            {
                Log.Warn("GiceController@Callback - State query paramater does not match session value, aborting!");
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
            string acess_token = model["access_token"].Value;

            //Decode the JWT Token.
            var account = new JwtSecurityToken(jwtEncodedString: acess_token).Payload;

            // Do we have an account for this GSF User?
            int gice_id = int.Parse(account["sub"].ToString());
            var waitlist_account = await _Db.Accounts.FindAsync(gice_id);
            if(waitlist_account != null)
            {
                // Update and save user account
                waitlist_account.Name = account["name"].ToString();
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
                    Id = int.Parse(account["sub"].ToString()),
                    Name = account["name"].ToString(),
                    RegisteredAt = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    LastLoginIP = _RequestorIP.MapToIPv4().ToString()
                };

                _Db.Add(waitlist_account);
                await _Db.SaveChangesAsync();
            }

            // Attempt to log the user in
            // For testing purposes redirect to view
            // With different information for login or failed login
            // TODO: Return redirect to index
            await LoginUserUsingId(waitlist_account.Id);

            return Redirect("~/");
            
        }

        [Authorize]
        public async Task<string> Logout()
        {
            Log.Info(string.Format("GiceController@Callback - Ended the authentication session for {0}", User.Identity.Name));

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

            // Look up the database for the account.
            // If no account is found fail the login and return.
            var account = await _Db.Accounts.FindAsync(id);
            if (account == null) return false;

            var claims = new List<Claim>
            {
                new Claim("id", account.Id.ToString()),
                new Claim("name", account.Name)
            };

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