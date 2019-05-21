using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class WaitingPilot
    {
        public int Id { get; set; }

        public int PilotId { get; set; }

        public int SystemId { get; set; }

        public int RemovedByAccountId { get; set; }

        public bool NewPilot { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        public Pilot Pilot { get; set; }

        public Account RemovedByAccount { get; set; }

        public ICollection<SelectedRole> SelectedRoles { get; set; }

        public ICollection<SelectedFits> SelectedFits { get; set; }
    }
}
