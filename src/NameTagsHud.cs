using System.Collections.Generic;
using System.Linq;
using HUD;

namespace NameYourFriends;

public class NameTagsHud(HUD.HUD hud, AbstractCreature abstractPlayer) : HudPart(hud)
{
    private readonly Dictionary<AbstractCreature, NameTagHudPart> _nameTags = [];
    public AbstractCreature AbstractPlayer => abstractPlayer;
    public RoomCamera Camera => abstractPlayer.world.game.cameras[0];
    
    public override void Update()
    {
        base.Update();

        var room = abstractPlayer?.Room;
        if (room == null)
        {
            ClearNameTags();
            return;
        }
        
        foreach (var creature in room.creatures
                     .Where(creature =>
                         creature.realizedCreature != null
                         && creature.NYF_HasName()
                         && !_nameTags.ContainsKey(creature)
                         )
                 )
        {
            _nameTags.Add(creature, new NameTagHudPart(this, creature));
        }
        
        foreach (var nameTag in _nameTags.ToList())
        {
            if (nameTag.Value.SlatedForDeletion)
            {
                nameTag.Value.ClearSprites();
                _nameTags.Remove(nameTag.Key);
            } else nameTag.Value.Update();
        }
    }
    
    public override void Draw(float timeStacker)
    {
        base.Draw(timeStacker);
        
        foreach (var nameTag in _nameTags.Values)
            nameTag.Draw(timeStacker);
    }
    
    public override void ClearSprites()
    {
        base.ClearSprites();
        ClearNameTags();
    }

    private void ClearNameTags()
    {
        foreach (var nameTag in _nameTags.Values)
            nameTag.ClearSprites();
        _nameTags.Clear();
    }
}