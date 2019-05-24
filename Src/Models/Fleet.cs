using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Fleet
    {
        public int Id { get; set; }

        public long EveFleetId { get; set; }

        [JsonIgnore]
        public int BackseatId { get; set; }
        [JsonIgnore]
        public int CommChannelId { get; set; }

        public int? SystemId { get; set; }

        public bool IsPublic { get; set; }

        public string Type { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Closed At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? ClosedAt { get; set; }

        // Navigation properties

        public CommChannel CommChannel { get; set; }

        public Pilot BossPilot { get; set; }

        public Account BackseatAccount { get; set; }

        public ICollection<FleetAssignment> FleetAssignments { get; set; }
    }
}
