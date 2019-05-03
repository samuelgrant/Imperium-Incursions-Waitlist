using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Corporation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [JsonIgnore]
        public int? AllianceId { get; set; }

        public string Name { get; set; }

        // Navigation properties

        public Alliance Alliance { get; set; }
        public ICollection<Pilot> Pilots { get; set; }
    }
}
