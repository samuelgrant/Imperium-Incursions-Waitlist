using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class CommChannel
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string LinkText { get; set; }

        // Navigation properties

        public ICollection<Fleet> Fleets { get; set; }
    }
}
