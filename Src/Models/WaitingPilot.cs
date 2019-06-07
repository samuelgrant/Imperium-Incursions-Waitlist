using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class WaitingPilot
    {
        public int Id { get; set; }

        public int PilotId { get; set; }

        [JsonIgnore]
        public int? SystemId { get; set; }

        [JsonIgnore]
        public int? RemovedByAccountId { get; set; }

        public bool NewPilot { get; set; }

        [JsonIgnore]
        [Display(Name = "Offline At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? OfflineAt { get; set; }

        [JsonIgnore]
        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public Pilot Pilot { get; set; }

        [JsonIgnore]
        public Account RemovedByAccount { get; set; }

        [JsonIgnore]
        public FleetAssignment FleetAssignment { get; set; }

        public StarSystem System { get; set; }

        public ICollection<SelectedRole> SelectedRoles { get; set; }

        public ICollection<SelectedFit> SelectedFits { get; set; }

        public string WaitingFor
        {
            get
            {
                TimeSpan span = (DateTime.UtcNow - CreatedAt);
                return $"{span.Hours.ToString()}H {span.Minutes.ToString()}M";
            }
        }

        [NotMapped]
        public bool IsOffline {
            get
            {
                if (OfflineAt == null)
                    return false;
                
                else if (OfflineAt.Value.AddMinutes(5) < DateTime.UtcNow)
                    return true;

                return false;
            }
            set
            {
                if (!value)
                {
                    OfflineAt = null;
                }
                else
                {
                    if (OfflineAt == null)
                        OfflineAt = DateTime.UtcNow;
                }
            }
        }
    }
}
