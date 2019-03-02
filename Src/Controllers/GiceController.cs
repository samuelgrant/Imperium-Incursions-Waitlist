using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Imperium_Incursions_Waitlist.Services;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using DotNetEnv;

namespace Imperium_Incursions_Waitlist.Controllers
{
    public class GiceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Initiates GICE SSO workflow
        /// </summary>
        public ActionResult Go()
        {
            Env.Load();

            //Get Client ID
            string clientID = Env.GetString("gice_clientID");


            //Callback
            string redirectUri = Url.Action("Callback", "Gice", null, "https").ToLower();


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
        public string Callback(string code, string state)
        {
            // Verify a code and state query parameter was returned.
            if (code == null || state == null)
            {
                Log.Error(string.Format("GiceController@Callback - Callback Error one or more query paramaters were missing\nState: {0}\nCode: {1}", state, code));
                return "Error: Authentication failed. Go back to try again!";
            }

            // Verify the state to protect against CSRF attacks.
            if (HttpContext.Session.GetString("state") != state)
            {
                Log.Warn("GiceController@Callback - State query paramater does not match session value, aborting!");
                HttpContext.Session.Remove("state");
                return "State miss match. Go back to try again!";
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
            var data = Encoding.Default.GetString(response);
            var model = JsonConvert.DeserializeObject<dynamic>(data);
            string acess_token = model["access_token"].Value;

            //Decode the JWT Token.
            var account = new JwtSecurityToken(jwtEncodedString: acess_token).Payload;

            return "Hi " + account["name"] + ". Your goonfleet ID is " + account["sub"] + ".";
        }

        public string Logout()
        {
            // Revoke the token
            // Logout 
            return "This will log a user out of our app and revoke GICE";
        }
    }
}