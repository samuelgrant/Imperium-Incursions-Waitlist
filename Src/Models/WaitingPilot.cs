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

        public int? SystemId { get; set; }

        public int? RemovedByAccountId { get; set; }

        public bool NewPilot { get; set; }

        [JsonIgnore]
        [Display(Name = "Offline At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? OfflineAt { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public Pilot Pilot { get; set; }

        public Account RemovedByAccount { get; set; }

        public FleetAssignment FleetAssignment { get; set; }

        public ICollection<SelectedRole> SelectedRoles { get; set; }

        public ICollection<SelectedFit> SelectedFits { get; set; }

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
