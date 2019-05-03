using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class AccountRole
    {
        [JsonIgnore]
        public int AccountId { get; set; }
        [JsonIgnore]
        public int RoleId { get; set; }

        // Navigation Properties
        public Account Account { get; set; }
        public Role Role { get; set; }
    }
}
