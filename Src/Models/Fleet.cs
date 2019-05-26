using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Fleet
    {
        public int Id { get; set; }

        public long EveFleetId { get; set; }

        [JsonIgnore]
        public int? BossPilotId { get; set; }

        [JsonIgnore]
        public int CommChannelId { get; set; }

        [JsonIgnore]
        public int? ErrorCount { get; set; }

        [JsonIgnore]
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

        public StarSystem System { get; set; }

        [NotMapped]
        public string MemberCount => GetOngridCount();

        /// <summary>
        /// Returns the number of pilots in the fleet who are not cynos
        /// </summary>
        /// <returns>Number of pilots in the fleet</returns>
        public string GetOngridCount()
        {
            int denominator = 0;


            if(Type == FleetType.Mothership.ToString())
            {
                denominator = 120;
            }
            else if(Type == FleetType.Headquarters.ToString())
            {
                denominator = 60;
            }
            else if(Type == FleetType.Assaults.ToString())
            {
                denominator = 30;
            }
            else if(Type == FleetType.Vanguards.ToString())
            {
                denominator = 15;
            }
            
            return $"0 / {denominator}";
        }
    }
}
