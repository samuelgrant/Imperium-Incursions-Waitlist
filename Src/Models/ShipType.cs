using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public enum Queue
    {
        // Todo: Put queue types here (may move to another file if needed)
    }

    public class ShipType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Queue Queue { get; set; }

        // Navigation properties

        public ICollection<Skill> Skills { get; set; }
        public ICollection<Fit> Fits { get; set; }
    }
}
