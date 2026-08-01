using System;
using System.Collections.Generic;

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
                    foreach (DataItemPart part in DataItemPlayer.main.ship.parts)
                    {
                        foreach (var ab in part.scriptable.abilities)
                        {
                            AddAbility(new Ability(ab,part));
                        }
                        if (part.scriptable.HasModifier())
                            racer.modifiers.Add(part.scriptable.GetInnateModifier(racer));
                    }
                    if (DataItemPlayer.main.ship.scriptable.HasModifier())
                        racer.modifiers.Add(DataItemPlayer.main.ship.scriptable.GetInnateModifier(racer));
                }
                else
                {
                    int level = TourneyController.main.GetCurrentRaceIndex();
                    AddAbility(new Ability(PartAbility.NpcWheel(level)));
                    if (level > 2 )
                    {
                        AddAbility(new Ability(PartAbility.NpcEngine(level-2)));
                    }
                }
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
        foreach (var ability in abilities)
        {
            if (!ability.Activate(racer, evt) && evt == ShipDefines.PartEvent.OnRaceStart)
                ability.FireCooldown(racer);
        }
    }
}
