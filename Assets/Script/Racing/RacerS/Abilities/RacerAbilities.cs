using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            case RaceDefines.RacePhase.RaceSetup:
                abilities.Clear();
                if (racer is PlayerRacer player)
                {
                    foreach (DataItemPart part in DataItemPlayer.main.car.parts)
                    {
                        AddPart(part);
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

                    int engLevel = level - RaceDefines.SeasonRaces + 1;

                    int numEngines = Mathf.FloorToInt(level / RaceDefines.SeasonRaces) ;
                    for (int i = 0; i < numEngines; i++)
                    {
                        rnVal = TourneyController.main.GetPlayerRival() == racer ? 1 : UnityEngine.Random.value;
                        AddAbility(new Ability(PartAbility.NpcEngine(engLevel, rnVal, 1f/numEngines), racer));
                    }
                }
                racer.modifiers.Refresh();
                racer.stats.UpdateGasTotal();
                fuel.SetLimit(racer.stats.gasTotal, Resource.LimitRule.full_value);
                break;
            case RaceDefines.RacePhase.RaceBegin:
                ListenToEvent(ShipDefines.PartEvent.OnRaceStart);
                break;
            case RaceDefines.RacePhase.RaceTick:
                ListenToEvent(ShipDefines.PartEvent.OnTimePass);
                break;
        }
        base.HandleRacePhase(phase);
    }

    public void AddPart(DataItemPart part)
    {
        AddPart(part.scriptable, part);
    }
    public void AddPart(PartScriptable scriptable, DataItemPart part = null)
    {
        foreach (var ab in scriptable.abilities)
        {
            AddAbility(new Ability(ab, part, racer));
        }
    }
    public void AddAbility(Ability ability)
    {
        abilities.Add(ability);
    }
    public void ListenToEvent(ShipDefines.PartEvent evt)
    {
        DataItemPlayer.main.Inspect($"{racer} activate ability {evt}");
        foreach (var ability in abilities)
        {
            if (!ability.Activate( evt) && evt == ShipDefines.PartEvent.OnRaceStart)
                ability.FireCooldown(racer.GetPropertyMultiplicative(ModifierDefines.Property.starting_cooldown_mult));
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
    public Ability[] GetAbilitiesCorrespondingToPart(DataItemPart part)
    {
        return abilities.Where(a => a.part == part).ToArray();
    }
}
