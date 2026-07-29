

using UnityEngine;

public class Ability : Countdown
{
    public PartAbility part;

    public Ability(PartAbility part)
    {
        this.part = part;
    }

    float GetFuelCost()
    {
        return part.fuelCost;
    }
    float GetPartCooldown()
    {
        return part.cooldown;
    }
    public bool RacerMeetsCondition(Racer racer)
    {
        switch (part.condition)
        {
            case ShipDefines.PartCondition.Random:
                return Random.value < part.conditionCheck;
            case ShipDefines.PartCondition.SpeedBelow:
                return racer.stats.realSpeed < part.conditionCheck;
            case ShipDefines.PartCondition.SpeedAbove:
                return racer.stats.realSpeed > part.conditionCheck;
            case ShipDefines.PartCondition.PositionAbove:
                return TourneyController.main.currentRace.GetPositionForRacer(racer) < part.conditionCheck;
            case ShipDefines.PartCondition.PositionBelow:
                return TourneyController.main.currentRace.GetPositionForRacer(racer) > part.conditionCheck;
            case ShipDefines.PartCondition.RelativeToRival:
                var rival = racer.GetRival();
                if (rival == null) return false;
                if (part.conditionCheck > 0 && TourneyController.main.currentRace.GetPositionForRacer(racer) > TourneyController.main.currentRace.GetPositionForRacer(rival))
                    return true;
                else if (part.conditionCheck < 0 && TourneyController.main.currentRace.GetPositionForRacer(racer) < TourneyController.main.currentRace.GetPositionForRacer(rival))
                    return true;
                else if (part.conditionCheck ==0 && Mathf.Abs( TourneyController.main.currentRace.GetPositionForRacer(racer) - TourneyController.main.currentRace.GetPositionForRacer(rival)) <=1)
                    return true;
                return false;
            case ShipDefines.PartCondition.GasAbove:
                return racer.abilities.fuel.GetValue() > part.conditionCheck;
            case ShipDefines.PartCondition.GasBelow:
                return racer.abilities.fuel.GetValue() < part.conditionCheck;
            case ShipDefines.PartCondition.GasPercentAbove:
                return racer.abilities.fuel.GetPercentage() > part.conditionCheck;
            case ShipDefines.PartCondition.GasPercentBelow:
                return racer.abilities.fuel.GetPercentage() < part.conditionCheck;
            case ShipDefines.PartCondition.LapAbove:
                return racer.position.currentLap > part.conditionCheck;
            case ShipDefines.PartCondition.LapBelow:
                return racer.position.currentLap < part.conditionCheck;
            default:
        return true;
    }
    }
    public bool CanBeActivated(Racer racer)
    {
        return racer.abilities.fuel.GetValue() >= GetFuelCost() && !IsRunning() && RacerMeetsCondition(racer) ;
    }
    public void ActivateOnRacer(Racer racer)
    {
        TourneyController.main.Inspect($"{racer} uses ability {part.InternalName} at {part.function}");
        part.action.GiveToPlayer(racer);
    }
    public void Activate(Racer racer, ShipDefines.PartEvent evt)
{
        if (evt == part.function && CanBeActivated(racer))
        {
            ActivateOnRacer(racer);
            racer.abilities.fuel.SubstractedValue(GetFuelCost());
            Set( GetPartCooldown());
        }
    }
}