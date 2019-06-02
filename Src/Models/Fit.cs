using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Fit
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int ShipTypeId { get; set; }

        public string FittingDNA { get; set; }

        public string Description { get; set; }

        public bool IsShipScan { get; set; }
        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Deleted At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties

        public Account Account { get; set; }

        public ShipType ShipType { get; set; }

        public ICollection<SelectedFit> SelectedFits { get; set; }
    }

    public struct FitDna{
        [NotMapped]
        public int ship_typeId;
        [NotMapped]
        public string dna;
        [NotMapped]
        public string description;
    }
}
