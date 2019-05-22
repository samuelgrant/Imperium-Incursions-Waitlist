using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public enum FleetType
    {
        // Put fleet types here - can shift enum to a different file if needed
    }


    public class Fleet
    {
        public int Id { get; set; }

        public int EveFleetId { get; set; }

        public int BossId { get; set; }

        public int BackseatId { get; set; }

        public int CommChannelId { get; set; }

        public int? SystemId { get; set; }

        public bool IsPublic { get; set; }

        public FleetType Type { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public CommChannel CommChannel { get; set; }

        public Pilot BossPilot { get; set; }

        public Account BackseatAccount { get; set; }

        public ICollection<FleetAssignment> FleetAssignments { get; set; }
    }
}
