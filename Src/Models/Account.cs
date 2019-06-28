using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool JabberNotifications { get; set; }

        [JsonIgnore]
        [Display(Name = "Login From IP")]
        [MaxLength(15)]
        public string LastLoginIP { get; set; }

        [Display(Name ="Registered At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name = "Last Login"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? LastLogin { get; set; }

        // Navigation Properties
        public ICollection<Pilot> Pilots { get; set; }
        [JsonIgnore]
        public ICollection<Ban> AccountBans { get; set; }
        [JsonIgnore]
        public ICollection<Ban> CreatedBans { get; set; }
        [JsonIgnore]
        public ICollection<Ban> UpdatedBans { get; set; }
        [JsonIgnore]
        public ICollection<Note> AccountNotes { get; set; }
        [JsonIgnore]
        public ICollection<Note> CreatedNotes { get; set; }
        [JsonIgnore]
        public ICollection<Note> UpdatedNotes { get; set; }

        public ICollection<AccountRole> AccountRoles { get; set; }
        [JsonIgnore]
        public ICollection<Fleet> BackseatedFleets { get; set; }
        [JsonIgnore]
        public ICollection<WaitingPilot> RemovedPilots { get; set; }
        [JsonIgnore]
        public ICollection<Fit> Fits { get; set; }

        public List<Fit> ActiveFits()
        {
            return Fits?.Where(f => f.DeletedAt == null && !f.IsShipScan).ToList();
        }
        // End of Navigation Properties


        /// <summary>
        /// Returns true if the account is related to 
        /// an active ban record.
        /// </summary>
        public bool IsBanned()
        {
            foreach (var ban in AccountBans)
            {
                if (ban.ExpiresAt == null)
                    return true;

                if (ban.ExpiresAt > DateTime.UtcNow)
                    return true;
            }

            return false;
        }
    }
}
