using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Services;
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

        public static void IsInDatabase(long id, Data.WaitlistDataContext _Db)
        {
            var corporation = _Db.Corporation.Find(id);
            if (corporation != null)
                return;

            var result = EsiWrapper.GetCorporation(id);

            corporation = new Corporation
            {
                Id = id,
                Name = result.Result.Name,
                AllianceId = result.Result.AllianceId 
            };

            //Corporation is not in an alliance
            if (corporation.AllianceId != 0)
                Alliance.IsInDatabase((int)corporation.AllianceId, _Db);

            _Db.Add(corporation);
            _Db.SaveChanges();

            return;
        }
    }
}
