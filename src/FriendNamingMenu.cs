using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx;
using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace NameYourFriends;

public class FriendNamingMenu : Dialog
{
    private readonly PauseMenu _owner;
    
    public FriendNamingMenu(ProcessManager manager, PauseMenu owner) : base(manager)
    {
        _owner = owner;
        _owner.container.alpha = 0f;
        
        pages.Add(new Page(this, null, "main", 0));
        
        var blackSprite = new FSprite("pixel")
        {
            color = MenuRGB(MenuColors.Black),
            scaleX = 1400f,
            scaleY = 800f,
            x = manager.rainWorld.options.ScreenSize.x / 2f,
            y = manager.rainWorld.options.ScreenSize.y / 2f,
            alpha = 0.25f
        };
        pages[0].Container.AddChild(blackSprite);
        
        var centerX =  manager.rainWorld.options.ScreenSize.x / 2f;
        var backButton = new SimpleButton(
            this,
            pages[0],
            Translate("BACK"),
            "BACK", 
            new Vector2(centerX - 100f, 50f),
            new Vector2(200f, 30f)
        );
        pages[0].subObjects.Add(backButton);

        var player = owner.game.FirstRealizedPlayer;
        var room = player?.room;
        if (room == null) return;
        
        List<AbstractCreature> nameableCreatures =
        [
            .. room.abstractRoom.creatures.Where(creature => creature.NYF_CanBeNamed(player))
        ];
        
        var wrapper = new MenuTabWrapper(this, pages[0]);
        pages[0].subObjects.Add(wrapper);

        var scrollbox = new OpScrollBox(
            new Vector2(centerX - 350f, 100f),
            new Vector2(700, manager.rainWorld.options.ScreenSize.y - 150f),
            nameableCreatures.Count * 40 + 10
        );
        pages[0].subObjects.Add(new UIelementWrapper(wrapper, scrollbox));

        const int sbCenterX = 350;
        var y = scrollbox.contentSize - 40;
        foreach (var creature in nameableCreatures)
        {
            var label = new OpLabel(
                new Vector2(sbCenterX - 70f, y),
                new Vector2(0f, 30f),
                $"{creature.creatureTemplate.name} ({creature.ID})",
                FLabelAlignment.Right
            ) { color = creature.NYF_GetColor() };
            scrollbox.AddItemToWrapped(label);

            var configurableName = new Configurable<string>(creature.NYF_GetName() ?? "");
            var nameEditBox = new OpTextBox(
                configurableName,
                new Vector2(sbCenterX - 30f, y),
                300f
            )
            {
                allowSpace = true,
                accept = OpTextBox.Accept.StringASCII
            };
            nameEditBox.OnValueUpdate += (config, value, _) =>
            {
                var filtered = NameYourFriends.FilterNameString(value);
                if (filtered != value) config.value = filtered;
                else creature.NYF_SetName(value);
            };
            scrollbox.AddItemToWrapped(nameEditBox);
            
            y -= 40;
        }

        if (nameableCreatures.Count == 0)
        {
            var label = new OpLabel(
                new Vector2(sbCenterX, y),
                new Vector2(0f, 30f),
                "No tamed creatures found in this room!",
                bigText: true
            );
            scrollbox.AddItemToWrapped(label);
        }
    }

    public override void Update()
    {
        base.Update();
        if (RWInput.CheckPauseButton(0))
        {
            Singal(backObject, "BACK");
        }
    }

    public override void Singal(MenuObject sender, string message)
    {
        if (message == "BACK")
        {
            _owner.container.alpha = 1;
            manager.StopSideProcess(this);
        }
    }
}