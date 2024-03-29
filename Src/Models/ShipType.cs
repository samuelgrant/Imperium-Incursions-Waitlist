﻿using Imperium_Incursions_Waitlist.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class ShipType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public Queue Queue { get; set; }

        [IntegerValidator(MinValue = 0, MaxValue = 10)]
        public int? HighSlots { get; set; }

        [IntegerValidator(MinValue = 0, MaxValue = 10)]
        public int? MidSlots { get; set; }

        [IntegerValidator(MinValue = 0, MaxValue = 10)]
        public int? LowSlots { get; set; }
        
        [IntegerValidator(MinValue = 0, MaxValue = 3)]
        public int? RigSlots { get; set; }


        // Navigation properties

        public ICollection<ShipSkill> ShipSkills { get; set; }
        public ICollection<Fit> Fits { get; set; }


        public static async 

        Task
EnsureInDatabase(int typeId, Data.WaitlistDataContext _Db)
        {
            ShipType ship = await _Db.ShipTypes.FindAsync(typeId);
            if (ship != null)
                return;

            var esiResponse = await EsiWrapper.GetShipTypeAsync(typeId);
            if (esiResponse.FirstOrDefault() == null)
                return;

            ship = new ShipType
            {
                Id = typeId,
                Name =  esiResponse[0].Name
            };

            await _Db.AddAsync(ship);
            await _Db.SaveChangesAsync();

            return;
        }
    }
}
