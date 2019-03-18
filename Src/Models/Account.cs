using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name ="Registered At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name = "Last Login"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "Login From IP")]
        public string LastLoginIP { get; set; }

        // Navigation Properties

        public ICollection<Pilot> Pilots { get; set; }        
        
        public ICollection<Ban> AccountBans { get; set; }

        public ICollection<Ban> CreatedBans { get; set; }

        public ICollection<Ban> UpdatedBans { get; set; }

        // End of Navigation Properties


        /// <summary>
        /// Returns true if the account is related to 
        /// an active ban record.
        /// </summary>
        public bool IsBanned()
        {
            throw new NotImplementedException();
        }

    }
}
