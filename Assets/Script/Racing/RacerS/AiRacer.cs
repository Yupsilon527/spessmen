using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiRacer : Racer
{
    Ability[] castables;
    public AiRacer(int rId) : base(rId)
    {
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        base.HandleRacePhase(phase);
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceSetup:
                castables = abilities.GetAbilities().Where(a => a.data.function == ShipDefines.PartEvent.OnActivated).ToArray();
                break;
            case RaceDefines.RacePhase.RaceTick:
                if (castables.Any(a=>a.CanBeActivated()))
                {
                    foreach (var c in castables)
                    {
                        if (ShouldUse(c))
                        {
                            c.Activate( ShipDefines.PartEvent.OnActivated);
                        }
                    }
                }
                break;
        }
    }
    bool ShouldUse(Ability a)
    {
        if (!a.CanBeActivated())
            return false;
        if (TourneyController.main.ongoingRace.GetPositionForRacer(this) == 0 && a.data.actions.Any(e => e.effectTarget == RaceDefines.AbilityTarget.FrontRacer))
            return false;
        if (abilities.fuel.GetPercentage() > DifficultyDefines.aiUseAbilityChance && a.data.actions.Any(e => e.value > 0 && e.stat == ShipDefines.StatType.FillGas))
            return false;
            return Random.value < DifficultyDefines.aiUseAbilityChance;
    }
}
