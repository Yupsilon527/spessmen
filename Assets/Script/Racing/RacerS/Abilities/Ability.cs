using UnityEngine;

public class Ability : Countdown
{
    public PartAbility part;

    float GetFuelCost()
    {
        return part.fuelCost;
    }
    float GetPartCooldown()
    {
        return part.cooldown;
    }
    public bool CanBeActivated(Racer racer)
    {
        return racer.fuel.GetValue() >= GetFuelCost() && !IsRunning();
    }
    public void Function(Racer racer)
    {
        switch (part.actions)
        {
            case ShipDefines.PartAction.GiveBaseSpeed:
                racer.stats.GiveBaseSpeed(part.abilityPower);
                break;
            case ShipDefines.PartAction.GiveBoostSpeed:
                racer.stats.GiveBoostSpeed(part.abilityPower);
                break;
            case ShipDefines.PartAction.GiveFuel:
                racer.fuel.GiveValue(part.abilityPower);
                break;
        }
    }
    public void Activate(Racer racer)
    {
        if (CanBeActivated(racer))
        {
            Function(racer);
            racer.fuel.SubstractedValue(GetFuelCost());
            Set( GetPartCooldown());
        }
    }
}