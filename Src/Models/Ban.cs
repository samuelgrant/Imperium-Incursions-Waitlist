using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Ban
    {
        public int Id { get; set; }
        public int Admin_Id { get; set; }
        public int Banned_Account_Id { get; set; }
        public string Reason { get; set; }
        public DateTime Issued_At { get; set; }
        public DateTime Expires_At { get; set; }
        public int Updated_By_Admin_Id { get; set; }
        public DateTime Updated_At { get; set; }
    }
}
