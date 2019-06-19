using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class SelectedRole
    {
        public int WaitingPilotId { get; set; }

        public int FleetRoleId { get; set; }

        // Navigation properties
        public WaitingPilot WaitingPilot { get; set; }

        public FleetRole FleetRole { get; set; }
    }
}
