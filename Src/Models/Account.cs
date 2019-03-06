using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist.Models
{
    public class Account
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTime Registered_At { get; set; }
        public DateTime Last_Login { get; set; }
        public string Last_Login_IP { get; set; }

        /// <summary>
        /// Returns true if the account is related to 
        /// an active ban record.
        /// </summary>
        public bool IsBanned()
        {
            throw new NotImplementedException();
        }

    }
}
