using System.IdentityModel.Tokens.Jwt;
using System.Net;
using DotNetEnv;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class EveController : Controller
    {
        /// <summary>
        /// Returns the Pilot Select View
        /// </summary>
        public IActionResult Index()
        {
            return View(viewName: "~/Views/PilotSelect.cshtml");
        }

        /// <summary>
        /// Initiates Eve SSO workflow
        /// </summary>
        public ActionResult Go()
        {
            Env.Load();

            //Get Client ID
            string clientID = Env.GetString("eve_clientID");

            //Callback
            string redirectUri = Url.Action("callback", "eve", null, protocol: "https").ToLower();


            //Create a state and save to session
            string newState = Util.RandomString(42);
            HttpContext.Session.SetString("state", newState);

            // Request Authentication from GICE.
            return Redirect(string.Format(
                "https://login.eveonline.com/v2/oauth/authorize?response_type={0}&redirect_uri={1}&client_id={2}&scope={3}&state={4}",
                "code",
                redirectUri,
                clientID,
                Env.GetString("scopes"),
                newState
            ));
        }

        /// <summary>
        /// Handles the GICE SSO callback.
        /// </summary>
        [ActionName("callback")]
        public IActionResult Callback(string code, string state)
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

            string access_token;
            string refresh_token;

            //Get the authorization code
            using (WebClient http = new WebClient())
            {
                string credentials = Util.Base64Encode(Env.GetString("eve_clientID") + ":" + Env.GetString("eve_clientSecret"));

                http.Headers.Add("Authorization", "Basic " + credentials);
                http.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                http.Headers.Add("Host", "login.eveonline.com");

                var response = http.UploadString("https://login.eveonline.com/v2/oauth/token", "POST", string.Format("grant_type={0}&code={1}",
                    "authorization_code",
                    code
                ));

                // Get the JWT Access token & Refresh Token
                var model = JsonConvert.DeserializeObject<dynamic>(response);
                access_token = model["access_token"].Value;
                refresh_token = model["refresh_token"].Value;
            }

            // Get vars from the access token
            var account = new JwtSecurityToken(jwtEncodedString: access_token).Payload;


            //return "Hi " + account["name"].ToString() + " your character ID is " + account["sub"].ToString().Split(':')[2];
            return null;
        }
    }
}