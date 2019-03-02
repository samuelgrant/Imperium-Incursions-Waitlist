using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Services
{
    public static class Util
    {

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="MaxCharacters">The max number of characters returned.</param>
        /// <returns></returns>
        public static string RandomString(int MaxCharacters)
        {
            //Generate string builder & a random object.
            StringBuilder builder = new StringBuilder();
            Random rnd = new Random();

            // Append characters to string builder until 
            // max the number of characters is reached
            char character;
            for (int i = 0; i < MaxCharacters; i++)
            {
                character = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
                builder.Append(character);
            }

            return builder.ToString().ToLower();
        }
    }
}
