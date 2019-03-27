using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Pilot
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        // EF Core recognizes this as FK automatically
        public int AccountId { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Corporation ID")]
        public long CorporationId { get; set; }

        [Display(Name = "ESI Token")]
        public string ESIToken { get; set; }

        [Display(Name = "Registered At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Account Account { get; set; }


        /// <summary>
        /// Checks to see if the account is linked
        /// </summary>
        public bool IsLinked()
        {
            return (AccountId.ToString() != null) ? true : false;
        }
    }
}
