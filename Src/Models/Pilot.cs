using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Pilot
    {
        public int Id { get; set; }
        public int Account_Id { get; set; }
        public string Name { get; set; }
        public int Corp_Id { get; set; }
        public string ESI_Token { get; set; }
        public DateTime Registered_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
