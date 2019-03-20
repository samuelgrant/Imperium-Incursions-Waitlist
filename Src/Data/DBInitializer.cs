using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;


namespace Imperium_Incursions_Waitlist.Data
{
    public class DBInitializer
    {
        /// <summary>
        /// Seeds the database for development purposes.
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(WaitlistDataContext context)
        {
            // Check for existing account records
            if (context.Accounts.Any())            
                return; // Already contains accounts            

            context.SaveChanges();
        }        
    }
}
