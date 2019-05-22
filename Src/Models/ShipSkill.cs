using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class ShipSkill
    {
        [JsonIgnore]
        public int ShipTypeId { get; set; }
        [JsonIgnore]
        public int SkillId { get; set; }

        public int Level { get; set; }

        public bool IsRequired { get; set; }

        // Navigation Properties
        public ShipType ShipType { get; set; }
        public Skill Skill { get; set; }
    }
}
