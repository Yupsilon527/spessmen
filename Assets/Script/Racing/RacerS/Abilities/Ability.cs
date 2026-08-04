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
        if (data.function == ShipDefines.PartEvent.OnActivated)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.active_fuel_consumtion);
        if (data.classification == ItemDefines.PartType.wheel)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_fuel_consumption);
        else if (data.classification == ItemDefines.PartType.gadget)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.gadget_fuel_consumtion);
        else if (data.classification == ItemDefines.PartType.nitro)
            baseCost *= racer.GetPropertyMultiplicative(ModifierDefines.Property.nitro_fuel_consumtion);
        return baseCost;
    }
    float GetPartCooldown(Racer racer)
    {
        var cooldown = data.cooldown * racer.GetPropertyMultiplicative(ModifierDefines.Property.cooldown_total);
        if (data.function == ShipDefines.PartEvent.OnActivated)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_cooldown);
        if (data.classification == ItemDefines.PartType.wheel)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.engine_cooldown);
        else if (data.classification == ItemDefines.PartType.nitro)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.nitro_cooldown);
        else if (data.classification == ItemDefines.PartType.gadget)
            cooldown *= racer.GetPropertyMultiplicative(ModifierDefines.Property.gadget_cooldown);
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
            TourneyController.main.Inspect($"{caster} uses ability {action.behavior} at {data.function} on {target}");

            if (!action.CanAffectRacer(target)) continue;

            float strength = racer.GetPropertyMultiplicative(ModifierDefines.Property.ability_power);

            if (target == racer)
            {
                if (action.stat == ShipDefines.StatType.BaseSpeed
                    || action.stat == ShipDefines.StatType.BoostSpeed
                    || action.stat == ShipDefines.StatType.TotalSpeed)
                {
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_total);
                    if (data.classification == ItemDefines.PartType.wheel)
                        strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_wheels);
                    else if (data.classification == ItemDefines.PartType.engine)
                        strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_engines);
                    else if (data.classification == ItemDefines.PartType.nitro)
                        strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_speed_nitro);
                }
                if (action.stat == ShipDefines.StatType.BaseSpeed)
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_base_speed_percentage);
                if (action.stat == ShipDefines.StatType.BoostSpeed)
                    strength *= caster.GetPropertyMultiplicative(ModifierDefines.Property.incoming_boost_speed_percentage);
            }
            else
            {
                strength *= target.GetPropertyMultiplicative(ModifierDefines.Property.effect_resistance);
                if (action.stat == ShipDefines.StatType.BaseSpeed || action.stat == ShipDefines.StatType.BoostSpeed || action.stat == ShipDefines.StatType.TotalSpeed)
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
            TourneyController.main.Inspect($"{racer} activates ability {data.InternalName}");

            ActivateOnRacer(racer);

            if (data.classification == ItemDefines.PartType.gadget)
                racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnGadgetActivate);
            else if (data.classification == ItemDefines.PartType.engine)
                racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnEngineActivate);
            else if (data.classification == ItemDefines.PartType.nitro)
                racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnNitroActivate);

            if (data.function == ShipDefines.PartEvent.OnActivated)
                if (data.cooldown > 1 || data.fuelCost > 20)
                    racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnBigAbilityActivate);
                else
                    racer.abilities.ListenToEvent(ShipDefines.PartEvent.OnFastAbilityActivate);

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