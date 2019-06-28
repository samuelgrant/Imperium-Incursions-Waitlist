using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Imperium_Incursions_Waitlist.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation properties
        public ICollection<Corporation> Corporations { get; set; }

        public static void EnsureInDatabase(int id, Data.WaitlistDataContext _Db)
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
