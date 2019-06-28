using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        /// Parses a Fit DNA Url and returns a fit strut with a typeId, dna and description
        /// </summary>
        /// <param name="fitUrl">Fit URL from in game chat. **USE A TRYCATCH around this call**</param>
        /// <returns>
        /// <see cref="Models.FitDna"/>
        /// </returns>
        public static Models.FitDna ParseFitDna(string fitUrl)
        {

            // If > comes before < then the user also submitted their name
            // Let's remove their name as we don't care about it.
            if(fitUrl.IndexOf('>') < fitUrl.IndexOf('<'))
                fitUrl = fitUrl.Substring(fitUrl.IndexOf('>') + 1);

            // Index 0 is the start of the URL
            int ship_typeId = int.Parse(fitUrl.Split(':')[1]);

            // Get the Fit DNA
            string fit_dna = ":";
            for (int i = 2; i < fitUrl.Split(':').Length - 2; i++)
                fit_dna = $"{fit_dna}{fitUrl.Split(':')[i]}:";

            // Get the fit Descrption
            int descriptionStartIndex = fitUrl.IndexOf("::") + 3;
            string fit_description = fitUrl.Substring(descriptionStartIndex, (fitUrl.IndexOf("</")) - descriptionStartIndex);
            return new Models.FitDna
            {
                ship_typeId = ship_typeId,
                dna = fit_dna,
                description = fit_description
            };
        }

        /// <summary>
        /// Creates a diffForHumans() like output that compares a historic time object against DateTime.UtcNow. Use this for pretty outputs on views.
        /// </summary>
        /// <param name="x">DateTime Object - Use DateTime.UtcNow</param>
        /// <returns>__H __M</returns>
        public static string WaitTime(DateTime x)
        {
            if (x == null) return "";

            TimeSpan timeSpan = DateTime.UtcNow - x;
            
            return $"{timeSpan.Hours}H {timeSpan.Minutes}M";
        }
    }
}
