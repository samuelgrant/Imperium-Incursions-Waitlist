using System;
using System.ComponentModel.DataAnnotations;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        public int CreatorAdminId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Message { get; set; }

        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Deleted At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Account CreatorAdmin { get; set; }

        public string TimeDiff()
        {
            TimeSpan duration = DateTime.UtcNow - CreatedAt;

            return string.Format("{0} {1} {2} minutes",
                duration.Days > 0 ? $"{duration.Days} days" : "",
                duration.Hours > 0 ? $"{duration.Hours} hours" : "",
                duration.Minutes
            );
        }
    }
}
