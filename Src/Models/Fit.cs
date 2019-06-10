using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Fit
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public int ShipTypeId { get; set; }

        public string FittingDNA { get; set; }

        public string Description { get; set; }

        public bool IsShipScan { get; set; }
        [Display(Name = "Created At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Deleted At"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties

        public ShipType ShipType { get; set; }

        public ICollection<SelectedFit> SelectedFits { get; set; }

        /// <summary>
        /// Returns a loadout of the ships fit for views
        /// </summary>
        /// <param name="db">**REQUIRES A DB CONTEXT from the calling controller**</param>
        /// <returns><see cref="FitLoadout"/></returns>
        //public async Task<FitLoadout> ModuleLoadout(Data.WaitlistDataContext _Db)
        //{
        //    Dictionary<ModuleItem, int> HighSlots = new Dictionary<ModuleItem, int>();
        //    Dictionary<ModuleItem, int> MidSlots = new Dictionary<ModuleItem, int>();
        //    Dictionary<ModuleItem, int> LowSlots = new Dictionary<ModuleItem, int>();
        //    Dictionary<ModuleItem, int> RigSlots = new Dictionary<ModuleItem, int>();
        //    Dictionary<ModuleItem, int> Unknown = new Dictionary<ModuleItem, int>();

        //    string[] modules = FittingDNA.Split(':');

        //    foreach (string x in modules)
        //    {
        //        //Deal with empty strings
        //        if (x == "")
        //            continue;

        //        ModuleItem item = await _Db.Modules.FindAsync(int.Parse(x.Split(';')[0]));
        //        // We don't know the module, let's create a placeholder
        //        if (item  == null)
        //        {
        //            Unknown.Add(new ModuleItem
        //                {
        //                    Id = int.Parse(x.Split(';')[0]),
        //                    Name = "Unknown",
        //                },
        //                int.Parse(x.Split(';')[1])
        //            );

        //            continue;
        //        }

        //        switch (item.Slot)
        //        {
        //            case "High":
        //                HighSlots.Add(item, int.Parse(x.Split(';')[1]));
        //                break;

        //            case "Mid":
        //                MidSlots.Add(item, int.Parse(x.Split(';')[1]));
        //                break;

        //            case "Low":
        //                LowSlots.Add(item, int.Parse(x.Split(';')[1]));
        //                break;

        //            case "Rig":
        //                RigSlots.Add(item, int.Parse(x.Split(';')[1]));
        //                break;

        //            default:
        //                Unknown.Add(item, int.Parse(x.Split(';')[1]));
        //                break;
        //        }

        //    }

            //return new FitLoadout
            //{
            //    HighSlots = HighSlots,
            //    MidSlots = MidSlots,
            //    LowSlots = LowSlots,
            //    RigSlots = RigSlots,
            //    Unknown = Unknown
            //};
        //}
    }

    public struct FitDna{
        [NotMapped]
        public int ship_typeId;
        [NotMapped]
        public string dna;
        [NotMapped]
        public string description;
    }

    //public struct FitLoadout
    //{
    //    public Dictionary<ModuleItem, int> HighSlots;
    //    public Dictionary<ModuleItem, int> MidSlots;
    //    public Dictionary<ModuleItem, int> LowSlots;
    //    public Dictionary<ModuleItem, int> RigSlots;
    //    public Dictionary<ModuleItem, int> Unknown;
    //}
}
