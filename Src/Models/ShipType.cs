using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        // Navigation properties

        public ICollection<ShipSkill> ShipSkills { get; set; }
        public ICollection<Fit> Fits { get; set; }
    }
}
