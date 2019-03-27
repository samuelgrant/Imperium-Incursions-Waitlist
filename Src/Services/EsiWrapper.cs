using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models;
using ESI.NET.Models.Character;
using Microsoft.Extensions.Options;

namespace Imperium_Incursions_Waitlist.Services
{
    public static class EsiWrapper
    {
        private static EsiClient s_client;
        public static void EnsureInit()
        {
            if(s_client == null)
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
                    Log.Error("Services/EsiWrapper@EnsureInit - Failed to setup an ESI Client: " + err.Message);
                }

            }
        }

        public static async Task<Information> GetPilot(int character_id)
        {
            EnsureInit();

            EsiResponse<Information> Character_response = await s_client.Character.Information(character_id);
            
            if(Character_response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error((string.Format("services/EsiWrapper@GetPilot - Error searching API: {0} {1}", Character_response.StatusCode, Character_response.Message)));

                return null;
            }

            return Character_response.Data;
        }
    }
}
