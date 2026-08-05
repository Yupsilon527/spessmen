using System;
using System.Collections.Generic;
using System.Linq;

public class RacerAbilities : RacerComponent
{
    [NonSerialized] protected List<Ability> abilities = new List<Ability>();
    public ResourceFloat fuel = new ResourceFloat(1, "gas", false, true);

    public RacerAbilities(Racer racer) : base(racer)
    {
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceBegin:
                abilities.Clear();
                if (racer is PlayerRacer player)
                {
                    foreach (DataItemPart part in DataItemPlayer.main.car.parts)
                    {
                        foreach (var ab in part.scriptable.abilities)
                        {
                            AddAbility(new Ability(ab,part, racer));
                        }
                        if (part.scriptable.HasModifier())
                            racer.modifiers.Add(part.scriptable.GetInnateModifier(racer));
                    }
                    if (DataItemPlayer.main.car.scriptable.HasModifier())
                        racer.modifiers.Add(DataItemPlayer.main.car.scriptable.GetInnateModifier(racer));
                }
                else
                {
                    float rnVal = TourneyController.main.GetPlayerRival() == racer ? 1f : (UnityEngine.Random.value * .75f + .25f);
                    int level = TourneyController.main.GetCurrentRaceIndex();
                    AddAbility(new Ability(PartAbility.NpcWheel(level, rnVal), racer));
                    if (level > 2 )
                    {
                        rnVal = TourneyController.main.GetPlayerRival() == racer ? 1 : UnityEngine.Random.value;
                        AddAbility(new Ability(PartAbility.NpcEngine(level-2, rnVal), racer));
                    }
                }
                racer.modifiers.Refresh();
                racer.stats.UpdateGasTotal();
                fuel.SetLimit(racer.stats.gasTotal, Resource.LimitRule.full_value);
                ListenToEvent(ShipDefines.PartEvent.OnRaceStart);
                break;
            case RaceDefines.RacePhase.RaceTick:
                ListenToEvent(ShipDefines.PartEvent.OnTimePass);
                break;
        }
        base.HandleRacePhase(phase);
    }
    void AddAbility(Ability ability)
    {
        abilities.Add(ability);
    }
    public void ListenToEvent(ShipDefines.PartEvent evt)
    {
        DataItemPlayer.main.Inspect($"{racer} activate ability {evt}");
        foreach (var ability in abilities)
        {
            if (!ability.Activate( evt) && evt == ShipDefines.PartEvent.OnRaceStart)
                ability.FireCooldown();
        }
    }
    public Ability[] GetAbilities()
    {
        return abilities.ToArray();
    }
    public Ability[] GetAbilityByType(ItemDefines.PartType type)
    {
        return abilities.Where(a => a.data.classification == type).ToArray();
    }
    public Ability[] GetAbilitiesCorrespondingToPart(PartScriptable part)
    {
        var valid = part.abilities.Select(p => p.InternalName);
        return abilities.Where(a => valid.Contains(a.data.InternalName)).ToArray();
    }
}
