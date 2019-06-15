using System;
using System.ComponentModel.DataAnnotations;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Note
    {
        public int Id { get; set; }

        public int AdminId { get; set; }

        public int TargetAccountId { get; set; }

        public int UpdatedByAdminId { get; set; }

        [Required]
        public string Message { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties

        public Account CreatorAdmin { get; set; }
        public Account UpdatingAdmin { get; set; }
        public Account TargetAccount { get; set; }

    }
}
