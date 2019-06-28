using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class PilotSkill
    {
        [JsonIgnore]
        public int PilotId { get; set; }
        [JsonIgnore]
        public int SkillId { get; set; }

        public int Level { get; set; }

        // Navigation Properties
        public Pilot Pilot { get; set; }
        public Skill Skill { get; set; }
    }
}
