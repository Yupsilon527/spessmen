

using UnityEngine;

public class Ability : Countdown
{
    public PartAbility data;

    public Ability(DataItemPart part)
    {
        this.data = part.scriptable.ability;
        part.correspondingAbility = this;
    }
    public Ability(PartAbility a)
    {
        this.data = a;
    }

    float GetFuelCost(Racer racer)
    {
        float baseCost = data.fuelCost * racer.GetPropertyMultiplicative(ModifierDefines.Property.fuel_consumption_total);
        if (data.classification == ItemDefines.PartType.wheel)
            baseCost*= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_fuel_consumption);
       else if (data.classification == ItemDefines.PartType.active)
            baseCost*= racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_fuel_consumtion);
        return baseCost;
    }
    float GetPartCooldown(Racer racer)
    {
        var cooldown = data.cooldown * racer.GetPropertyMultiplicative(ModifierDefines.Property.cooldown_total);
        if (data.classification == ItemDefines.PartType.wheel)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_cooldown);
        else if (data.classification == ItemDefines.PartType.active)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_cooldown);
        return cooldown;
    }
    public bool RacerMeetsCondition(Racer racer)
    {
        switch (data.condition)
        {
            case ShipDefines.PartCondition.Random:
                return Random.value < data.conditionCheck;
            case ShipDefines.PartCondition.SpeedBelow:
                return racer.stats.realSpeed < data.conditionCheck;
            case ShipDefines.PartCondition.SpeedAbove:
                return racer.stats.realSpeed > data.conditionCheck;
            case ShipDefines.PartCondition.PositionAbove:
                return TourneyController.main.ongoingRace.GetPositionForRacer(racer) < data.conditionCheck;
            case ShipDefines.PartCondition.PositionBelow:
                return TourneyController.main.ongoingRace.GetPositionForRacer(racer) > data.conditionCheck;
            case ShipDefines.PartCondition.RelativeToRival:
                var rival = racer.GetRival();
                if (rival == null) return false;
                if (data.conditionCheck > 0 && TourneyController.main.ongoingRace.GetPositionForRacer(racer) > TourneyController.main.ongoingRace.GetPositionForRacer(rival))
                    return true;
                else if (data.conditionCheck < 0 && TourneyController.main.ongoingRace.GetPositionForRacer(racer) < TourneyController.main.ongoingRace.GetPositionForRacer(rival))
                    return true;
                else if (data.conditionCheck ==0 && Mathf.Abs( TourneyController.main.ongoingRace.GetPositionForRacer(racer) - TourneyController.main.ongoingRace.GetPositionForRacer(rival)) <=1)
                    return true;
                return false;
            case ShipDefines.PartCondition.GasAbove:
                return racer.abilities.fuel.GetValue() > data.conditionCheck;
            case ShipDefines.PartCondition.GasBelow:
                return racer.abilities.fuel.GetValue() < data.conditionCheck;
            case ShipDefines.PartCondition.GasPercentAbove:
                return racer.abilities.fuel.GetPercentage() > data.conditionCheck;
            case ShipDefines.PartCondition.GasPercentBelow:
                return racer.abilities.fuel.GetPercentage() < data.conditionCheck;
            case ShipDefines.PartCondition.LapAbove:
                return racer.position.currentLap > data.conditionCheck;
            case ShipDefines.PartCondition.LapBelow:
                return racer.position.currentLap < data.conditionCheck;
            default:
        return true;
    }
    }
    public bool CanBeActivated(Racer racer)
    {
        return racer.abilities.fuel.GetValue() >0 && !IsRunning() && RacerMeetsCondition(racer) ;
    }
    public void ActivateOnRacer(Racer racer)
    {
        TourneyController.main.Inspect($"{racer} uses ability {data.InternalName} at {data.function}");

        float strength = racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_power);

        if (data.action.stat ==  PlayerStatsAlteration.StatType.BaseSpeed
            || data.action.stat == PlayerStatsAlteration.StatType.BoostSpeed)
        {
            strength *= racer.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_total);
            if (data.classification == ItemDefines.PartType.wheel)
                strength *= racer.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_wheels);
            else if (data.classification == ItemDefines.PartType.engine)
                strength *= racer.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_engines);
        }
        if (data.action.stat == PlayerStatsAlteration.StatType.BaseSpeed)
            strength *= racer.GetPropertyMultiplicative(ModifierDefines.Property.incoming_base_speed_percentage);
        if (data.action.stat == PlayerStatsAlteration.StatType.BoostSpeed)
            strength *= racer.GetPropertyMultiplicative(ModifierDefines.Property.incoming_boost_speed_percentage);


        data.action.GiveToPlayer(racer, strength);
    }
    public bool Activate(Racer racer, ShipDefines.PartEvent evt)
{
        if (evt == data.function && CanBeActivated(racer))
        {
            ActivateOnRacer(racer);
            racer.abilities.fuel.SubstractedValue(GetFuelCost(racer));
            FireCooldown(racer);
            return true;
        }
        return false;
    }
    public void FireCooldown(Racer racer)
    {
        Set(GetPartCooldown(racer));
    }
}