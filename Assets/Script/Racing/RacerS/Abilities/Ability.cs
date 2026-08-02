


public class Ability : Countdown
{
    public PartAbility data;

    public Ability(PartAbility a, DataItemPart part)
    {
        data = a;
        part.correspondingAbility = this;
    }
    public Ability(PartAbility a)
    {
        data = a;
    }

    float GetFuelCost(Racer racer)
    {
        float baseCost = data.fuelCost * racer.GetPropertyMultiplicative(ModifierDefines.Property.fuel_consumption_total);
        if (data.classification == ItemDefines.PartType.wheel)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_fuel_consumption);
        else if (data.classification == ItemDefines.PartType.gadget)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.gadget_fuel_consumtion);
        return baseCost;
    }
    float GetPartCooldown(Racer racer)
    {
        var cooldown = data.cooldown * racer.GetPropertyMultiplicative(ModifierDefines.Property.cooldown_total);
        if (data.classification == ItemDefines.PartType.wheel)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_cooldown);
        else if (data.classification == ItemDefines.PartType.gadget)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_cooldown);
        return cooldown;
    }
    public bool CanBeActivated(Racer racer)
    {
        return (data.fuelCost == 0 || racer.abilities.fuel.GetValue() > 0) && !IsRunning() && ShipDefines.RacerMeetsCondition(racer, data.condition, data.conditionCheck);
    }
    public void ActivateOnRacer(Racer racer)
    {
        TourneyController.main.Inspect($"{racer} uses ability {data.InternalName} at {data.function}");


        foreach (ConditionalPartAltetration action in data.actions)
        {
            var caster = RaceDefines.GetRacerRelative(racer, action.effectSource);
            var target = RaceDefines.GetRacerRelative(racer, action.effectTarget);

            if (!action.CanAffectRacer(target)) continue;

            float strength = racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_power);

            if (target == racer)
            {
                if (action.stat == PlayerStatsAlteration.StatType.BaseSpeed
                    || action.stat == PlayerStatsAlteration.StatType.BoostSpeed
                    || action.stat == PlayerStatsAlteration.StatType.TotalSpeed)
                {
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_total);
                    if (data.classification == ItemDefines.PartType.wheel)
                        strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_wheels);
                    else if (data.classification == ItemDefines.PartType.engine)
                        strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_engines);
                }
                if (action.stat == PlayerStatsAlteration.StatType.BaseSpeed)
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_base_speed_percentage);
                if (action.stat == PlayerStatsAlteration.StatType.BoostSpeed)
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_boost_speed_percentage);
            }
            else
            {
                strength *= target.GetPropertyMultiplicative(ModifierDefines.Property.effect_resistance);
                if (action.stat == PlayerStatsAlteration.StatType.BaseSpeed || action.stat == PlayerStatsAlteration.StatType.BoostSpeed || action.stat == PlayerStatsAlteration.StatType.TotalSpeed)
                {
                    strength *= target.GetPropertyMultiplicative(ModifierDefines.Property.speed_resistance);
                }
            }
            action.GiveToPlayer(racer, target, strength);
        }
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