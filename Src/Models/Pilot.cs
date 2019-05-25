using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Imperium_Incursions_Waitlist.Services;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Pilot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CharacterID { get; set; }

        // EF Core recognizes this as FK automatically
        public int AccountId { get; set; }

        [Required]
        public string CharacterName { get; set; }

        [Display(Name = "Corporation ID")]
        [JsonIgnore]
        [ForeignKey("Corporation")]
        public long CorporationID { get; set; }

        [Display(Name = "Refresh Token")]
        [JsonIgnore]
        public string RefreshToken { get; set; }

        [Display(Name = "Access Token")]
        [JsonIgnore]
        public string Token { get; set; }

        [NotMapped]
        public bool ESIValid
        {
            get => RefreshToken != null;
        }

        [Display(Name = "Registered At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime RegisteredAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Account Account { get; set; }
        public Corporation Corporation { get; set; }
        public ICollection<PilotSkill> PilotSkills { get; set; }
        public ICollection<Fleet> OwnedFleets { get; set; }


        /// <summary>
        /// Checks to see if the account is linked
        /// </summary>
        public bool IsLinked() => AccountId.ToString() != null;
        
        /// <summary>
        /// Checks to see if the pilot belongs to a specific ID
        /// </summary>
        /// <param name="accountId">The ID of the account to check against</param>
        /// <returns></returns>
        public bool BelongsToAccount(int accountId) => AccountId == accountId;
    }
}
