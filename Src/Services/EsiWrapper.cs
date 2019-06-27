using System;
using System.Threading.Tasks;
using System.Net;
using ESI.NET;
using ESI.NET.Models;
using ESI.NET.Enumerations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using DotNetEnv;
using ESI.NET.Models.SSO;
using System.Collections.Generic;

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
                        AuthVersion = AuthVersion.v2
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
        /// Resolves a Type ID to a ship using ESI
        /// </summary>
        public static async Task<List<ESI.NET.Models.Universe.ResolvedInfo>> GetShipTypeAsync(int typeId)
        {
            EnsureInit();
            List<long> ids = new List<long>
            {
                typeId
            };
            EsiResponse<List<ESI.NET.Models.Universe.ResolvedInfo>> resolvedInfo_response =await s_client.Universe.Names(ids);

            if(resolvedInfo_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error resolving IDs to names'{1}': {2}", resolvedInfo_response.StatusCode, resolvedInfo_response.Endpoint, resolvedInfo_response.Message);
                return null;
            }

            return resolvedInfo_response.Data;
        }

        /// <summary>
        /// Returns an ESI Client
        /// </summary>
        /// <returns></returns>
        public static EsiClient GetEsiClient()
        {
            EnsureInit();

            return s_client;
        }

        public static async Task<ESI.NET.Models.Location.Location> GetSystem(AuthorizedCharacterData pilot)
        {
            EsiClient x = GetEsiClient();
            x.SetCharacterData(pilot);
            EsiResponse<ESI.NET.Models.Location.Location> Location_response = await x.Location.Location();

            if(Location_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", Location_response.StatusCode, Location_response.Endpoint, Location_response.Message);
                return null;
            }


            return Location_response.Data;
        }

        public static async Task<List<ESI.NET.Models.Fleets.Member>> GetFleetMembers(AuthorizedCharacterData pilot, long fleet_id)
        {
            EsiClient esi = GetEsiClient();
            esi.SetCharacterData(pilot);

            EsiResponse<System.Collections.Generic.List<ESI.NET.Models.Fleets.Member>> FleetMembers_response = await esi.Fleets.Members(fleet_id);

            if(FleetMembers_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", FleetMembers_response.StatusCode, FleetMembers_response.Endpoint, FleetMembers_response.Message);
                throw new Exception(FleetMembers_response.ErrorType().ToString());
            }

            return FleetMembers_response.Data;
        }

        public static async Task<ESI.NET.Models.Universe.SolarSystem> GetSystemInformation(int SystemId)
        {
            EnsureInit();

            EsiResponse<ESI.NET.Models.Universe.SolarSystem> SystemInfo_response = await s_client.Universe.System(SystemId);

            if(SystemInfo_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", SystemInfo_response.StatusCode, SystemInfo_response.Endpoint, SystemInfo_response.Message);
                return null;
            }

            return SystemInfo_response.Data;
        }

        public static async Task<SearchResults> Search(string searchTerm, bool strict, SearchCategory category)
        {
            EnsureInit();
            
            EsiResponse<SearchResults> Search_response = await s_client.Search.Query(SearchType.Public, searchTerm, category, strict);

            if (Search_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", Search_response.StatusCode, Search_response.Endpoint, Search_response.Message);
                return null;
            }

            return Search_response.Data;
        }

        /// <summary>
        /// Checks ESI to see if the pilot is online.
        /// </summary>
        /// <param name="pilot">Pilot explicit cast as AuthorizedCharacterData</param>
        /// <returns>Bool</returns>
        public static async Task<bool> IsOnlineAsync(AuthorizedCharacterData pilot)
        {
            EsiClient x = GetEsiClient();
            x.SetCharacterData(pilot);
            EsiResponse<ESI.NET.Models.Location.Activity> Location_response = await x.Location.Online();

            if (Location_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", Location_response.StatusCode, Location_response.Endpoint, Location_response.Message);
                return true;
            }

            return Location_response.Data.Online;
        }

        public static async Task<int[]> GetAllSystemIds()
        {
            EnsureInit();

            EsiResponse<int[]> SystemsArray_response = await s_client.Universe.Systems();

            if (SystemsArray_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error searching API '{1}': {2}", SystemsArray_response.StatusCode, SystemsArray_response.Endpoint, SystemsArray_response.Message);
                return null;
            }

            return SystemsArray_response.Data;
        }

        /// <summary>
        /// Sets a new autopilot destination in game.
        /// </summary>
        /// <param name="pilot">Pilot explicit cast as AuthorizedCharacterData</param>
        /// <param name="system_id">ID of the target solar system.</param>
        /// <see cref="ESI.NET.Models.Universe.SolarSystem"/>
        public static void SetDestination(AuthorizedCharacterData pilot, int system_id)
        {
            try
            {
                EsiClient x = GetEsiClient();
                x.SetCharacterData(pilot);
                x.UserInterface.Waypoint(system_id, true, true);
            }
            catch(Exception ex)
            {
                Console.Beep();
            }
        }

        /// <summary>
        /// Opens the Show Info window for a Character, Corporation or Alliance in game.
        /// </summary>
        /// <param name="pilot">Pilot explicit cast as AuthorizedCharacterData</param>
        /// <param name="target_id"></param>
        /// <see cref="Models.Pilot"/>
        /// <seealso cref="AuthorizedCharacterData"/>
        /// <seealso cref="ESI.NET.Models.Character"/>
        /// <seealso cref="ESI.NET.Models.Corporation"/>
        /// <seealso cref="ESI.NET.Models.Alliance"/>
        public static void ShowInfo(AuthorizedCharacterData pilot, int target_id)
        {
            try
            {
                EsiClient x = GetEsiClient();
                x.SetCharacterData(pilot);
                x.UserInterface.Information(target_id);
            } 
            catch(Exception ex)
            {
                Console.Beep();
            }
        }

        internal static async Task<ESI.NET.Models.Character.Information> PilotLookupAsync(int characterId)
        {
            EnsureInit();

            try
            {
                EsiResponse<ESI.NET.Models.Character.Information> Character_response = await s_client.Character.Information(characterId);
                if (Character_response.StatusCode != HttpStatusCode.OK)
                {
                    s_Log.LogError("{0} error searching API '{1}': {2}", Character_response.StatusCode, Character_response.Endpoint, Character_response.Message);
                    return null;
                }

                return Character_response.Data;
            }
            catch (Exception ex)
            {
                s_Log.LogError($"Error getting character information {ex.Message}");
                return null;
            }
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

            try
            {
                EsiResponse<ESI.NET.Models.Corporation.Corporation> Corporation_response = await s_client.Corporation.Information((int)id);

                if (Corporation_response.StatusCode != HttpStatusCode.OK)
                {
                    s_Log.LogError("{0} error searching API '{1}': {2}", Corporation_response.StatusCode, Corporation_response.Endpoint, Corporation_response.Message);
                    return null;
                }

                return Corporation_response.Data;
            }
            catch(Exception ex)
            {
                s_Log.LogError($"Error getting corporation information {ex.Message}");
                return null;
            }
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

            try
            {
                EsiResponse<ESI.NET.Models.Alliance.Alliance> Alliance_response = await s_client.Alliance.Information(id);

                if (Alliance_response.StatusCode != HttpStatusCode.OK)
                {
                    s_Log.LogError("{0} error search API '{1}': {2}", Alliance_response.StatusCode, Alliance_response.Endpoint, Alliance_response.Message);
                    return null;
                }

                return Alliance_response.Data;
            }
            catch(Exception ex)
            {
                s_Log.LogError($"Error getting alliance information {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Invites the pilot to a fleet. If a specific squad is not specified they will be invited to the default squad.
        /// </summary>
        /// <param name="fleetBoss">Fleet Boss explicit cast as AuthorizedCharacterData</param>
        /// <param name="fleetId">Target fleet Id</param>
        /// <param name="squadId">Target squad Id</param>
        /// <param name="pilotId">Invitee Id</param>
        public static async Task<object> FleetInvite(AuthorizedCharacterData fleetBoss, long fleetId, long squadId, long wingId, int pilotId)
        {
            try
            {                
                EsiClient x = GetEsiClient();
                x.SetCharacterData(fleetBoss);
                EsiResponse<string> response = await x.Fleets.InviteCharacter(fleetId, pilotId, ESI.NET.Enumerations.FleetRole.SquadMember, wingId, squadId);
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    s_Log.LogError("{0} error search API '{1}': {2}", response.StatusCode, response.Endpoint, response.Message);

                    Exception ex;
                    if (response.Message.Contains("'error_label': 'FleetCandidateOffline', 'error_dict'"))
                    {
                        ex = new Exception("Fleet Candidate Offline");
                    } 
                    else
                    {
                        ex = new Exception(response.Message);
                    }

                    throw new Exception("Fleet Invite Failed", ex);
                }

                return null;
            }
            catch (Exception ex)
            {
                //s_Log.LogError("{0} error searching API '{1}': {2}", FleetMembers_response.StatusCode, FleetMembers_response.Endpoint, FleetMembers_response.Message);
                //throw new Exception(FleetLayout_response.ErrorType().ToString());
                throw new NotImplementedException();
            }
        }

        public static async Task<List<ESI.NET.Models.Fleets.Wing>> GetFleetLayout(AuthorizedCharacterData fleetBoss, long fleetId)
        {
            EsiClient x = GetEsiClient();
            x.SetCharacterData(fleetBoss);
            EsiResponse<List<ESI.NET.Models.Fleets.Wing>> FleetLayout_response = await x.Fleets.Wings(fleetId);

            if (FleetLayout_response.StatusCode != HttpStatusCode.OK)
            {
                s_Log.LogError("{0} error search API '{1}': {2}", FleetLayout_response.StatusCode, FleetLayout_response.Endpoint, FleetLayout_response.Message);
                throw new Exception(FleetLayout_response.ErrorType().ToString());
            }

            return FleetLayout_response.Data;
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
