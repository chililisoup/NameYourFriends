using System;
using System.Security.Permissions;
using BepInEx;
using BepInEx.Logging;
using NameYourFriends.Hooks;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace NameYourFriends;

[BepInPlugin(ModID, ModName, ModVersion)]
public class NameYourFriends : BaseUnityPlugin
{
    public const string ModID = "chililisoup.nameyourfriends";
    private const string ModName = "Name Your Friends";
    private const string ModVersion = "1.0";
    private new static ManualLogSource Logger { get; set; }
    
    public void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        Logger = base.Logger;
    }
    
    private bool _isInit;
    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (_isInit) return;
            
            ModConfig.Init();
            CreatureStateHooks.PatchHooks();
            UIHooks.PatchHooks();

            _isInit = true;
        }
        catch (Exception e)
        {
            Logger.LogError($"{ModName} failed to load!");
            Logger.LogError(e);
        }
    }
}
