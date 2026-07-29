using System;
using System.Collections;
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
                if (racer is PlayerRacer player)
                {
                    abilities.Clear();
                    foreach (DataItemPart part in DataItemPlayer.main.ship.parts)
                    {
                        abilities.Add(new Ability(part.GetAbility()));
                    }
                }
                fuel.SetLimit(racer.stats.gasTotal, Resource.LimitRule.full_value);
                break;
            case RaceDefines.RacePhase.RaceTick:
                ListenToEvent(ShipDefines.PartEvent.OnTimePass);
                break;
        }
        base.HandleRacePhase(phase);
    }
    public void ListenToEvent(ShipDefines.PartEvent evt)
    {
        foreach (var ability in abilities)
        {
            ability.Activate(racer, evt);
        }
    }
}
