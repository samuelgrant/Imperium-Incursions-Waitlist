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
        public static void Initialize(WaitlistDataContext context)
        {
            char separator = Path.DirectorySeparatorChar;

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

            // Seeds comms channels -- June 2019
            if (!context.CommChannels.Any())
            {
                var comms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CommChannel>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "CommChannels.json"));
                context.CommChannels.AddRange(comms);
                context.SaveChanges();
            }

            // Seeds default account roles required to access all modules of the application
            if (!context.Roles.Any())
            {
                var roles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Role>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "Roles.json"));
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // Seeds fleet roles that can be selected by the pilot
            if (!context.FleetRoles.Any())
            {
                var fleetRoles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FleetRole>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "FleetRoles.json"));
                context.FleetRoles.AddRange(fleetRoles);
                context.SaveChanges();
            }

            // Seeds solar systems from the SDE
            if (!context.Systems.Any())
            {
                var systems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StarSystem>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "Systems.json"));
                context.Systems.AddRange(systems);
                context.SaveChanges();
            }

            // Seeds ship types from the SDE -- 2017
            if (!context.ShipTypes.Any())
            {
                var shipTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShipType>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "ShipTypes.json"));
                context.ShipTypes.AddRange(shipTypes);
                context.SaveChanges();
            }

            // Seed module items from the SDE -- June 2019
            if (!context.Modules.Any())
            {
                var modules = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ModuleItem>>(File.ReadAllText($"Data{separator}Seeds" + Path.DirectorySeparatorChar + "Modules.json"));
                context.AddRange(modules);
            }

            context.SaveChanges();
        }
    }
}
