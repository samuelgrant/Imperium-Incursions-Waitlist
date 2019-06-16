using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Imperium_Incursions_Waitlist.Services;
using ESI.NET;
using ESI.NET.Models.SSO;
using ESI.NET.Enumerations;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Pilot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CharacterID { get; set; }

        // EF Core recognizes this as FK automatically
        public int? AccountId { get; set; }

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

        public static explicit operator AuthorizedCharacterData(Pilot v)
        {
            return new AuthorizedCharacterData
            {
                AllianceID = 0,
                CharacterID = v.CharacterID,
                CharacterName = v.CharacterName,
                CharacterOwnerHash = "",
                ExpiresOn = DateTime.UtcNow.AddMinutes(30),
                FactionID = 0,
                RefreshToken = v.RefreshToken,
                Token = v.Token,
                TokenType = "Character",
                Scopes = ""
            };
        }

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
        public bool IsLinked() => AccountId != null;
        
        /// <summary>
        /// Checks to see if the pilot belongs to a specific ID
        /// </summary>
        /// <param name="accountId">The ID of the account to check against</param>
        /// <returns></returns>
        public bool BelongsToAccount(int accountId) => AccountId == accountId;

        public async Task UpdateToken()
        {
            EsiClient s_client = EsiWrapper.GetEsiClient();

            try
            {
                SsoToken token = await s_client.SSO.GetToken(GrantType.RefreshToken, RefreshToken);
                RefreshToken = token.RefreshToken;
                Token = token.AccessToken;
            } 
            catch(Exception ex)
            {
                Console.Beep();
            }
        }
    }
}
