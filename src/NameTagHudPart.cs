using RWCustom;
using UnityEngine;

namespace NameYourFriends;

public class NameTagHudPart
{
    private readonly NameTagsHud _owner;
    private readonly AbstractCreature _creature;
    private readonly FSprite _arrow;
    private readonly FSprite _gradient;
    private readonly FLabel _label;
    private Vector2 _pos;
    private Vector2 _lastPos;
    private float _alpha;
    private float _lastAlpha;
    public bool SlatedForDeletion;
    
    public NameTagHudPart(NameTagsHud owner, AbstractCreature creature)
    {
        _creature = creature;
        _owner = owner;
        _pos = new Vector2(-1000f, -1000f);
        _lastPos = _pos;
        _alpha = -ModConfig.NameTagHideDuration.Value;
        _lastAlpha = _alpha;

        var color = creature.NYF_GetColor();
        
        _gradient = new FSprite("Futile_White")
        {
            alpha = 0f,
            x = -1000f,
            color = Color.black,
            shader = owner.hud.rainWorld.Shaders["FlatLight"]
        };
        owner.hud.fContainers[0].AddChild(_gradient);

        _label = new FLabel(Custom.GetFont(), creature.NYF_GetName() ?? creature.ID.ToString())
        {
            alpha = 0f,
            x = -1000f,
            color = color
        };
        owner.hud.fContainers[0].AddChild(_label);
        
        _arrow = new FSprite("Multiplayer_Arrow")
        {
            alpha = 0f,
            x = -1000f,
            color = color
        };
        owner.hud.fContainers[0].AddChild(_arrow);
    }

    public void Update()
    {
        var creature = _creature.realizedCreature;
        var player = _owner.RealizedPlayer;
        var name = creature.abstractCreature.NYF_GetName();
        if (creature.slatedForDeletetion || creature.room != player.room || name == null)
        {
            SlatedForDeletion = true;
            return;
        }
        
        _lastPos = _pos;
        _lastAlpha = _alpha;
        _label.text = name;
        
        var chunk = creature.mainBodyChunk;
        _pos = chunk.pos + new Vector2(0, chunk.rad + 40f) - _owner.Camera.pos;
        
        var fade = player.inShortcut || player.input[0].AnyInput;
        _alpha = Mathf.Clamp(
            _alpha + (fade ? -0.1F : 0.05F), 
            -ModConfig.NameTagHideDuration.Value,
            creature.dead ? 0.5f : 1f
        );
    }

    public void Draw(float timeStacker)
    {
        var pos = Vector2.Lerp(_lastPos, _pos, timeStacker) + new Vector2(0.01f, 0.01f);
        var alpha = Mathf.Pow(Mathf.Max(0.0f, Mathf.Lerp(_lastAlpha, _alpha, timeStacker)), 0.7f);
        
        _arrow.SetPosition(pos);
        _arrow.alpha = alpha;

        _gradient.SetPosition(pos.x, pos.y + 10f);
        _gradient.scale = Mathf.Lerp(80f, 110f, alpha) / 16f;
        _gradient.alpha = 0.17f * Mathf.Pow(alpha, 2f);
        
        _label.SetPosition(pos.x, pos.y + 20f);
        _label.alpha = alpha;
    }
    
    public void ClearSprites()
    {
        _arrow.RemoveFromContainer();
        _gradient.RemoveFromContainer();
        _label.RemoveFromContainer();
    }
}