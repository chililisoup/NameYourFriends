using System.Linq;
using Menu;
using UnityEngine;

namespace NameYourFriends.Hooks;

public static class UIHooks
{
    public static void PatchHooks()
    {
        On.HUD.HUD.InitSinglePlayerHud += AddNameTagsToHUD;
        On.Menu.PauseMenu.ctor += AddPauseMenuButton;
        On.Menu.PauseMenu.Singal += OnPauseMenuSignal;
    }

    private static void AddNameTagsToHUD(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD hud, RoomCamera cam)
    {
        orig(hud, cam);
        
        if (hud.owner is Player player)
        {
            hud.AddPart(new NameTagsHud(hud, player.abstractCreature));
        }
    }
    
    private static void AddPauseMenuButton(On.Menu.PauseMenu.orig_ctor orig, PauseMenu self, ProcessManager manager, RainWorldGame game)
    {
        orig(self, manager, game);
        if (game.IsArenaSession || game.FirstRealizedPlayer?.room == null)
            return;

        var nyfMenuButton = new SimpleButton(
            self,
            self.pages[0],
            "FRIEND NAMING",
            "ENTER_NYF_MENU",
            NameYourFriends.DressMySlugcatEnabled ?
                new Vector2(self.exitButton.pos.x, self.exitButton.pos.y + 38f) :
                new Vector2(self.continueButton.pos.x, self.continueButton.pos.y + 38f),
            new Vector2(110f, 30f)
        );
        self.pages[0].subObjects.Add(nyfMenuButton);
        
        if (NameYourFriends.DressMySlugcatEnabled)
        {
            nyfMenuButton.nextSelectable[3] = self.exitButton;
            self.exitButton.nextSelectable[1] = nyfMenuButton;
        }
        else
        {
            nyfMenuButton.nextSelectable[3] = self.continueButton;
            self.continueButton.nextSelectable[1] = nyfMenuButton;
        }
    }
    
    private static void OnPauseMenuSignal(On.Menu.PauseMenu.orig_Singal orig, PauseMenu self, MenuObject sender, string message)
    {
        orig(self, sender, message);
        if (message == "ENTER_NYF_MENU")
            self.manager.ShowDialog(new FriendNamingMenu(self.manager, self));
    }
}