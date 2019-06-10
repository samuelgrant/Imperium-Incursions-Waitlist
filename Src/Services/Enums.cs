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

    public enum FleetErrorType
    {
        FleetDead,
        InvalidBoss
    };
}
