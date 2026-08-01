using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace NameYourFriends;

internal static class CreatureExtensions
{
    private static readonly ConditionalWeakTable<AbstractCreature, string> CreatureNameTable = new();

    [CanBeNull]
    public static string NYF_GetName(this AbstractCreature creature)
    {
        return creature != null && CreatureNameTable.TryGetValue(creature, out var name) ?
            name : null;
    }

    public static void NYF_SetName(this AbstractCreature creature, [CanBeNull] string name)
    {
        if (creature == null) return;
        CreatureNameTable.Remove(creature);
        if (name != null) CreatureNameTable.Add(creature, name);
    }

    public static bool NYF_HasName(this AbstractCreature creature)
    {
        return creature.NYF_GetName() != null;
    }

    public static bool NYF_CanBeNamed(this AbstractCreature creature, Creature namer)
    {
        return ModConfig.UnrestrictedNaming.Value ?
            !namer.room?.game.Players.Contains(creature) ?? false :
            creature.NYF_HasName() || creature.abstractAI?.RealAI?.friendTracker?.friend == namer;
    }

    public static Color NYF_GetColor(this AbstractCreature creature)
    {
        var color = creature.realizedCreature.ShortCutColor();
        var maxColorComponent = color.maxColorComponent * 0.8f;
        return maxColorComponent > 0f
            ? new Color(
                color.r / maxColorComponent,
                color.g / maxColorComponent,
                color.b / maxColorComponent
            ) : Color.white;
    }
}
