using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Services
{
    public static class Util
    {
        static readonly char[] padding = { '=' };
        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="maxCharacters">The max number of characters returned.</param>
        /// <returns></returns>
        public static string RandomString(int maxCharacters)
        {
            //Generate string builder & a random object.
            StringBuilder builder = new StringBuilder();
            Random rnd = new Random();

            // Append characters to string builder until 
            // max the number of characters is reached
            char character;
            for (int i = 0; i < maxCharacters; i++)
            {
                character = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
                builder.Append(character);
            }

            return builder.ToString().ToLower();
        }

        /// <summary>
        /// Encodes a string to Base64 format.
        /// </summary>
        /// <param name="_string"></param>
        /// <returns>_string in base64 format</returns>
        public static string Base64Encode(string _string){
            var bytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_string);

            return System.Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Verifies a JWT token is valid a dictionary an array of claims.
        /// </summary>
        /// <param name="OpenIdDomain">Top level domain of the OpenID Connect provider</param>
        /// <param name="OpenIdAudiance">fill me out later</param>
        /// <param name="accessToken">fill me out later</param>
        /// <returns>A key value pair dictionary of claims</returns>
        public static async Task<Dictionary<string, string>> JwtVerify(string OpenIdDomain, string OpenIdAudiance, string accessToken)
        {
            // If either of the paramters were missing abort.
            if (OpenIdDomain == null || OpenIdAudiance == null)
                return null;

            var s_Log = Services.ApplicationLogging.CreateLogger("Services.Util");
            SecurityToken validatedToken;

            try
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationBuilder =
                    new ConfigurationManager<OpenIdConnectConfiguration>($"{OpenIdDomain}.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());

                OpenIdConnectConfiguration openIdConfig = await configurationBuilder.GetConfigurationAsync(CancellationToken.None);

                TokenValidationParameters ValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = openIdConfig.Issuer,
                    ValidAudience = OpenIdAudiance, 
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var user = handler.ValidateToken(accessToken, ValidationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                s_Log.LogWarningFormat("Error validating JWT Token {0}", ex.Message);
                return null;
            };

#pragma warning disable IDE0019 // Use pattern matching
            JwtSecurityToken jwt = validatedToken as JwtSecurityToken;
#pragma warning restore IDE0019 // Use pattern matching

            if (jwt == null)
            {
                s_Log.LogWarningFormat("Authentication failed a JWT was not found");
                return null;
            }

            Dictionary<string, string> Claims = new Dictionary<string, string>();
            foreach(var claim in jwt.Claims)
            {
                Claims.Add(claim.Type, claim.Value);
            }

            return Claims;
        } 
    }
}
