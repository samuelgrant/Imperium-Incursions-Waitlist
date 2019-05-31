using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class FleetAssignment
    {
        public int? WaitingPilotId { get; set; }

        public int FleetId { get; set; }

        public int CurrentShipId { get; set; }

        public bool IsExitCyno { get; set; }

        public bool TakesFleetWarp { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Deleted At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties

        public Fleet Fleet { get; set; }

        public WaitingPilot WaitingPilot { get; set; }
    }
}
