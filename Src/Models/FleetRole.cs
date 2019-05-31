using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class FleetRole
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool Avaliable { get; set; }
        // Navigation properties

        public ICollection<SelectedRole> SelectedRoles { get; set; }
    }
}
