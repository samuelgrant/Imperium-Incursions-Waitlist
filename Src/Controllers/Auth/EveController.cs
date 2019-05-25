using System;
using DotNetEnv;
using ESI.NET.Enumerations;
using ESI.NET.Models.SSO;
using Imperium_Incursions_Waitlist.Models;
using Imperium_Incursions_Waitlist.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class EveController : Controller
    {
        private ILogger _Logger;
        private Data.WaitlistDataContext _Db;

        public EveController(Data.WaitlistDataContext db, ILogger<EveController> logger)
        {
            _Db = db;
            _Logger = logger;
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
        public async System.Threading.Tasks.Task<IActionResult> Callback(string code, string state)
        {
            // Verify a code and state query parameter was returned.
            if (code == null || state == null)
            {
                _Logger.LogWarning("Eve Callback Error: One or more of the query parameters are missing. State: {0}. Code: {1}", state, code);
                return StatusCode(452);
            }

            // Verify the state to protect against CSRF attacks.
            if (HttpContext.Session.GetString("state") != state)
            {
                _Logger.LogWarning("Eve Callback Error: Invalid state returned.");
                HttpContext.Session.Remove("state");
                return StatusCode(452);
            }

            // Clear the state session
            HttpContext.Session.Remove("state");


            ESI.NET.EsiClient s_client = EsiWrapper.GetEsiClient();

            SsoToken token = await s_client.SSO.GetToken(GrantType.AuthorizationCode, code);
            AuthorizedCharacterData n_pilot = await s_client.SSO.Verify(token);


            long corporation_id = (long)n_pilot.CorporationID;

            Corporation.EnsureInDatabase(corporation_id, _Db);

            var pilot = await _Db.Pilots.FindAsync(n_pilot.CharacterID);
            if(pilot == null)
            {
                // User doesn't exist, create a new account
                pilot = new Pilot()
                {
                    CharacterID = n_pilot.CharacterID,
                    AccountId = User.AccountId(),
                    CharacterName = n_pilot.CharacterName,
                    CorporationID = corporation_id,
                    RefreshToken = n_pilot.RefreshToken,
                    Token = n_pilot.Token,
                    UpdatedAt = DateTime.UtcNow,
                    RegisteredAt = DateTime.UtcNow,
                };

                _Db.Add(pilot);
                await _Db.SaveChangesAsync();

                _Logger.LogDebug("{0} has linked the pilot {1} to their account.", User.FindFirst("name").Value, pilot.CharacterName);

                //TODO: alert user that it worked
                return Redirect("/pilot-select");
            }
            else if (!pilot.IsLinked() || pilot.BelongsToAccount(int.Parse(User.FindFirst("id").Value)))
            {
                // Update the pilot information - This may include a new account if it was unlinked.
                pilot.AccountId = User.AccountId();
                pilot.CharacterName = n_pilot.CharacterName;
                pilot.CorporationID = corporation_id;
                pilot.RefreshToken = n_pilot.RefreshToken;
                pilot.Token = n_pilot.Token;
                pilot.UpdatedAt = DateTime.UtcNow;

                _Db.Update(pilot);
                await _Db.SaveChangesAsync();

                _Logger.LogDebug("{0} has updated the pilot {1} that is linked to their account.", User.FindFirst("name").Value, pilot.CharacterName);

                //TODO: alert user that it worked
                return Redirect("/pilot-select");
            }

            //TODO: alert user that it failed
            _Logger.LogDebug("{0} has tried to link {1} to their account, however it is linked to someone else’s account.");
            return Redirect("/pilot-select");
        }
    }
}