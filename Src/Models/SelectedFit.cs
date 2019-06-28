using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class SelectedFit
    {
        public int WaitingPilotId { get; set; }

        public int FitId { get; set; }

        public bool ToFleet { get; set; }

        public WaitingPilot WaitingPilot { get; set; }

        public Fit Fit { get; set; }
    }
}
