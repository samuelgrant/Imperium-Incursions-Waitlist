using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        // Navigation properties

        public ICollection<PilotSkill> PilotSkills { get; set; }
        public ICollection<ShipSkill> ShipSkills { get; set; }
    }
}
