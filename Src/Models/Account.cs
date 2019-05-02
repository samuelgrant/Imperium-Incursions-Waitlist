using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        [JsonIgnore]
        [Display(Name = "Login From IP")]
        public string LastLoginIP { get; set; }

        // Navigation Properties
        public ICollection<Pilot> Pilots { get; set; }
        [JsonIgnore]
        public ICollection<Ban> AccountBans { get; set; }
        [JsonIgnore]
        public ICollection<Ban> CreatedBans { get; set; }
        [JsonIgnore]
        public ICollection<Ban> UpdatedBans { get; set; }
        
        public ICollection<AccountRole> AccountRoles { get; set; }

        // End of Navigation Properties


        /// <summary>
        /// Returns true if the account is related to 
        /// an active ban record.
        /// </summary>
        public bool IsBanned()
        {
            //if (AccountBans == null)
            //    return false;

            foreach (var ban in AccountBans)
            {
                if (ban.ExpiresAt == null || ban.ExpiresAt > DateTime.UtcNow)
                    return true;
            }

            return false;
        }

    }
}
