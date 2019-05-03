using Imperium_Incursions_Waitlist.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        // Navigation properties

        public Corporation Corporation { get; set; }

        public static void IsInDatabase(int id, Data.WaitlistDataContext _Db)
        {
            var alliance = _Db.Alliance.Find(id);
            if (alliance != null)
                return;

            var result = EsiWrapper.GetAlliance(id);

            alliance = new Alliance
            {
                Id = id,
                Name = result.Result.Name
            };

            _Db.Add(alliance);
            _Db.SaveChanges();

            return;
        }
    }
}
