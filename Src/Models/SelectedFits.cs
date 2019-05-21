using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class SelectedFits
    {
        public int WaitingPilotId { get; set; }

        public int FitId { get; set; }

        public bool ToFleet { get; set; }

        // Navigation properties

        public WaitingPilot WaitingPilot { get; set; }

        public Fit Fit { get; set; }
    }
}
