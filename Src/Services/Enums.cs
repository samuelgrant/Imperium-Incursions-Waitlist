using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imperium_Incursions_Waitlist
{
    enum FleetType {
        Headquarters,
        Assaults,
        Vanguards,
        Mothership
    };

    public enum Queue
    {
        None,
        DPS,
        Logi,
        Capital,
        Fax,
        Support
    };
}
