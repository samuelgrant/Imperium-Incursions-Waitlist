using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Ban
    {
        public int Id { get; set; }
        
        public int AdminId { get; set; }
        
        public int? UpdatedByAdminId { get; set; }        
        
        public int BannedAccountId { get; set; }

        public string Reason { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Expires At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? ExpiresAt { get; set; }        

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties

        /* EF Core gets confused when we try to
         * define multiple FKs to the same parent
         * table, using data annotations. Therefore,
         * these relationships are defined in the 
         * data context class, using Fluent API.
         */
        public Account BannedAccount { get; set; }
        public Account CreatorAdmin { get; set; }
        public Account UpdatingAdmin { get; set; }


        // End of Navigation Properties

        /// <summary>
        /// Takes an HTTP form value for ban duration
        /// and creates a ban expiry object
        /// </summary>
        /// <param name="banDuration"></param>
        /// <returns>DateTime or null</returns>
        /// <see cref="DateTime.UtcNow"/>
        public static DateTime? BanExpiryDate(string banDuration)
        {
            int days = int.Parse(banDuration);

            if (days > 0)
                return DateTime.UtcNow.AddDays(days);

            return null;
        }
    }
}
