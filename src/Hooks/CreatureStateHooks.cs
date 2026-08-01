namespace NameYourFriends.Hooks;

public static class CreatureStateHooks
{
    public static void PatchHooks()
    {
        On.CreatureState.BaseSaveString += CreatureBaseSaveString;
        On.CreatureState.LoadFromString += CreatureLoadFromString;
    }
    
    private static string CreatureBaseSaveString(On.CreatureState.orig_BaseSaveString orig, CreatureState self)
    {
        var str = orig(self);
        var nyfName = self.creature.NYF_GetName();
        return nyfName != null ? $"{str}NYF_Name<cC>{nyfName}<cB>" : str;
    }
    
    private static void CreatureLoadFromString(On.CreatureState.orig_LoadFromString orig, CreatureState self, string[] s)
    {
        orig(self, s);

        if (!self.unrecognizedSaveStrings.ContainsKey("NYF_Name")) return;
        self.creature.NYF_SetName(self.unrecognizedSaveStrings["NYF_Name"]);
        self.unrecognizedSaveStrings.Remove("NYF_Name");
    }
}