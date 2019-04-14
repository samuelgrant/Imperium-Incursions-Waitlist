using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Name { get; set; }

        // Navigation properties
        [JsonIgnore]
        public ICollection<AccountRole> AccountRoles { get; set; }
    }
}
