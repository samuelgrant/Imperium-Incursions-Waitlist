using System;
using System.Linq;
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

            var accounts = new Account[]
            {
                new Account {Id = 1, Name = "SamJ", RegisteredAt = DateTime.Now},
                new Account {Id = 2, Name = "MitchQ", RegisteredAt = DateTime.Now},
                new Account {Id = 3, Name = "DanteG", RegisteredAt = DateTime.Now}
            };

            foreach(Account account in accounts)            
                context.Accounts.Add(account);
            
            context.SaveChanges();
        }        
    }
}
