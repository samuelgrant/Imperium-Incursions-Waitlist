using System;
using System.Threading.Tasks;
using System.Net;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.Character;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

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
                    IOptions<EsiConfig> options = Options.Create<EsiConfig>(new EsiConfig()
                    {
                        EsiUrl = "https://esi.evetech.net/",
                        DataSource = DataSource.Tranquility,
                        UserAgent  = "Imperium Incursions Waitlist. Contact Caitlin Viliana"
                    });

                    s_client = new EsiClient(options);
                }
                catch (Exception err)
                {
                    s_Log.LogError("Error initializing ESI Client: {0}", err.Message);
                }
            }
        }

        public static async Task<Information> GetPilot(int character_id)
        {
            EnsureInit();

            EsiResponse<Information> Character_response = await s_client.Character.Information(character_id);
            
            if(Character_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{2}': {1}", Character_response.StatusCode, Character_response.Message, Character_response.Endpoint);
                return null;
            }

            return Character_response.Data;
        }
    }
}
