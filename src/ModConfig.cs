namespace NameYourFriends;

public static class ModConfig
{
    public static Configurable<bool> UnrestrictedNaming;
    public static Configurable<int> NameTagHideDuration;
    
    public static void Init()
    {
        var oi = MachineConnector.GetRegisteredOI(NameYourFriends.ModID);
        var config = oi.config;

        UnrestrictedNaming = config.Bind(
            "UnrestrictedNaming",
            false,
            new ConfigurableInfo("Allows naming any creature in the room regardless of whether it was tamed", autoTab:"Main")
        );
        
        NameTagHideDuration = config.Bind(
            "NameTagHideDuration",
            3,
            new ConfigurableInfo(
                "How long you have to be still for name tags to appear; -1 (left) makes them always visible",
                new ConfigAcceptableRange<int>(-1, 10),
                "Main"
            )
        );
    }
}