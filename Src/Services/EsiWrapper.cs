using System;
using System.Threading.Tasks;
using System.Net;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.Character;
using ESI.NET.Models.Corporation;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using DotNetEnv;

namespace Imperium_Incursions_Waitlist.Services
{
    public static class EsiWrapper
    {
        private static EsiClient s_client;
        private static ILogger s_Log;

        public static void EnsureInit()
        {
            s_Log = Services.ApplicationLogging.CreateLogger("EsiWrapper");

            if (s_client == null)
            {
                try
                {
                    Env.Load();

                    IOptions<EsiConfig> options = Options.Create<EsiConfig>(new EsiConfig()
                    {
                        EsiUrl = "https://esi.evetech.net/",
                        DataSource = DataSource.Tranquility,
                        UserAgent = "Imperium Incursions Waitlist. Contact Caitlin Viliana",
                        ClientId = Env.GetString("eve_clientID"),
                        SecretKey = Env.GetString("eve_clientSecret"),
                    });

                    s_client = new EsiClient(options);
                }
                catch (Exception err)
                {
                    s_Log.LogError("Error initializing ESI Client: {0}", err.Message);
                }
            }
        }

        /// <summary>
        /// Requests a character's information through ESI
        /// route /character/{character_id}/
        /// </summary>
        /// <param name="character_id">Target character's ID</param>
        /// <see cref="ESI.NET.Models.Character"/>
        /// <returns>ESI Character Model</returns>
        public static async Task<Information> GetPilot(int character_id)
        {
            EnsureInit();

            EsiResponse<Information> Character_response = await s_client.Character.Information(character_id);

            if (Character_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", Character_response.StatusCode, Character_response.Endpoint, Character_response.Message);
                return null;
            }

            return Character_response.Data;
        }

        /// <summary>
        /// Requests a corporation's information through ESI
        /// </summary>
        /// <param name="id">Target corporation's ID</param>
        /// <see cref="ESI.NET.Models.Corporation.Corporation"/>
        /// <returns>ESI Corporation Model</returns>
        public static async Task<ESI.NET.Models.Corporation.Corporation> GetCorporation(long id)
        {
            EnsureInit();
            
            EsiResponse<ESI.NET.Models.Corporation.Corporation> Corporation_response = await s_client.Corporation.Information((int)id);

            if(Corporation_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", Corporation_response.StatusCode, Corporation_response.Endpoint, Corporation_response.Message);
                return null;
            }

            return Corporation_response.Data;

        }

        /// <summary>
        /// Request an alliance's information through ESI
        /// </summary>
        /// <param name="id">Target alliance Id</param>
        /// <see cref="ESI.NET.Models.Alliance.Alliance"/>
        /// <returns>ESI Aliance Model</returns>
        public static async Task<ESI.NET.Models.Alliance.Alliance> GetAlliance(int id)
        {
            EnsureInit();

            EsiResponse<ESI.NET.Models.Alliance.Alliance> Alliance_response = await s_client.Alliance.Information(id);

            if(Alliance_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error search API '{1}': {2}", Alliance_response.StatusCode, Alliance_response.Endpoint, Alliance_response.Message);
                return null;
            }

            return Alliance_response.Data;
        }
    }
}
