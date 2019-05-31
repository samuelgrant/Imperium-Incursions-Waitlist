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

        /// <summary>
        /// Returns the number of pilots allowed on grid. 
        ///  This is based on the fleet type selected by the FC.
        /// </summary>
        /// <returns>int</returns>
        public int GetFleetTypeMax()
        {
            if (Type == FleetType.Mothership.ToString())
            {
                return 120;
            }
            else if (Type == FleetType.Headquarters.ToString())
            {
                return 60;
            }
            else if (Type == FleetType.Assaults.ToString())
            {
                return 30;
            }
            else if (Type == FleetType.Vanguards.ToString())
            {
                return 15;
            }

            return 0;
        }

        /// <summary>
        /// Returns the number of "on grid" pilots. 
        /// An on grid pilot is any pilot in fleet who is not marked as an exit cyno.
        /// </summary>
        /// <param name="FleetPilots">
        /// Must be passed in manually by the controller method
        ///  as this property cannot be found inside the fleet object.
        /// </param>
        /// <returns>int FleetPilots</returns>
        public int GetOngridCount(List<FleetAssignment> FleetPilots)
        {
            int onGrid = 0;

            foreach (FleetAssignment x in FleetPilots)
                if (!x.IsExitCyno && x.DeletedAt == null)
                    onGrid++;

            return onGrid;
        }
    }
}
