using System;
using System.Collections.Generic;
using System.IO;
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
        public static async System.Threading.Tasks.Task Initialize(WaitlistDataContext context)
        {
            // Check for existing account records
            if (!context.Accounts.Any())
            {
                // No Accounts in database, seeding basic accounts.
                var accounts = new Account[]
                {
                new Account {Id = 1, Name = "SamJ", RegisteredAt = DateTime.Now},
                new Account {Id = 2, Name = "MitchQ", RegisteredAt = DateTime.Now},
                new Account {Id = 3, Name = "DanteG", RegisteredAt = DateTime.Now}
                };

                foreach (Account account in accounts)
                    context.Accounts.Add(account);
            }

            // Check for existing Comms Channel records
            if (!context.CommChannels.Any())
            {
                // No channels in database, seeding basic channels
                var CommChannels = new CommChannel[]
                {
                    new CommChannel {
                        LinkText = "Inc -> Fleet A",
                        Url = "mumble://mumble.goonfleet.com/Squads%20and%20SIGs/Incursions/Fleet%20A?title=Goonfleet&version=1.2.0"
                    },
                    new CommChannel {
                        LinkText = "Inc -> Fleet B",
                        Url = "mumble://mumble.goonfleet.com/Squads%20and%20SIGs/Incursions/Fleet%20B?title=Goonfleet&version=1.2.0"
                    },
                    new CommChannel {
                        LinkText = "Inc -> Fleet C",
                        Url = "mumble://mumble.goonfleet.com/Squads%20and%20SIGs/Incursions/Fleet%20C?title=Goonfleet&version=1.2.0"
                    },
                    new CommChannel {
                        LinkText = "Inc -> Fleet D",
                        Url = "mumble://mumble.goonfleet.com/Squads%20and%20SIGs/Incursions/Fleet%20D?title=Goonfleet&version=1.2.0"
                    }
                };

                foreach (CommChannel channel in CommChannels)
                    context.CommChannels.Add(channel);
            }

            // Seeds solar systems from the SDE
            if (!context.Systems.Any())
            {
                var systems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StarSystem>>(File.ReadAllText("Data" + Path.DirectorySeparatorChar + "Systems.json"));
                context.Systems.AddRange(systems);
            }

            // Seeds ship types from the SDE -- 2017
            if(!context.ShipTypes.Any())
            {
                var shipTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShipType>>(File.ReadAllText("Data" + Path.DirectorySeparatorChar + "ShipTypes.json"));
                context.ShipTypes.AddRange(shipTypes);
            }

            context.SaveChanges();
        }        
    }
}
