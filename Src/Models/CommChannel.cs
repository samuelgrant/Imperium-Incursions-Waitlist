using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class CommChannel
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public string LinkText { get; set; }

        // Navigation properties

        public ICollection<Fleet> Fleets { get; set; }
    }
}
