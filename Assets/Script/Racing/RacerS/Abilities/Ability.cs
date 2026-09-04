using UnityEngine;

public class Ability : Countdown
{
    int useCount = 0;
    public Racer caster;
    public PartAbility data;


    public float grantedSpeed, grantedFuel = 0;

    public Ability(PartAbility a, DataItemPart part, Racer caster)
    {
        data = a;
        this.caster = caster;
    }
    public Ability(PartAbility a, Racer caster)
    {
        data = a;
        this.caster = caster;
    }

    public float GetFuelCost()
    {
        float baseCost = data.fuelCost * caster.GetPropertyMultiplicative(ModifierDefines.Property.fuel_consumption_total);
        if (data.function == ShipDefines.PartEvent.OnActivated)
            baseCost *= caster.GetPropertyMultiplicative(ModifierDefines.Property.active_fuel_consumtion);
        if (data.classification == ItemDefines.PartType.wheel)
            baseCost *= caster.GetPropertyMultiplicative(ModifierDefines.Property.engine_fuel_consumption);
        else if (data.classification == ItemDefines.PartType.gadget)
            baseCost *= caster.GetPropertyMultiplicative(ModifierDefines.Property.gadget_fuel_consumtion);
        else if (data.classification == ItemDefines.PartType.nitro)
            baseCost *= caster.GetPropertyMultiplicative(ModifierDefines.Property.nitro_fuel_consumtion);
        return baseCost;
    }
    float GetPartCooldown()
    {
        var cooldown = data.cooldown * caster.GetPropertyMultiplicative(ModifierDefines.Property.cooldown_total);
        if (data.function == ShipDefines.PartEvent.OnActivated)
            cooldown *= caster.GetPropertyMultiplicative(ModifierDefines.Property.ability_cooldown);
        if (data.classification == ItemDefines.PartType.wheel)
            cooldown *= caster.GetPropertyMultiplicative(ModifierDefines.Property.engine_cooldown);
        else if (data.classification == ItemDefines.PartType.nitro)
            cooldown *= caster.GetPropertyMultiplicative(ModifierDefines.Property.nitro_cooldown);
        else if (data.classification == ItemDefines.PartType.gadget)
            cooldown *= caster.GetPropertyMultiplicative(ModifierDefines.Property.gadget_cooldown);
        return cooldown;
    }
    public bool CanBeActivated()
    {
        return (data.fuelCost == 0
            || (data.overflow == ItemDefines.OverflowBehavior.unusable && caster.abilities.fuel.GetValue() >= GetFuelCost())
            || (data.overflow != ItemDefines.OverflowBehavior.unusable && caster.abilities.fuel.GetValue() > 0))
            // || caster.abilities.fuel.GetValue() >= GetFuelCost()) 
            && (data.maxUses == 0 || useCount < data.maxUses) && !IsRunning()
            && ShipDefines.RacerMeetsCondition(caster, data.condition, data.conditionCheck);
    }
    public void ActivateOnRacer(float strength = 1)
    {
        Use();
        TourneyController.main.Inspect($"{caster} uses ability {data.InternalName} at {data.function}");
        foreach (ConditionalPartAltetration action in data.actions)
        {
            var caster = RaceDefines.GetRacerRelative(this.caster, action.effectSource);
            var target = RaceDefines.GetRacerRelative(this.caster, action.effectTarget);
            TourneyController.main.Inspect($"{caster} uses ability {action.behavior} at {data.function} on {target}");

            if (!action.CanAffectRacer(target)) continue;

             strength *= this.caster.GetPropertyMultiplicative(ModifierDefines.Property.ability_power);

            if (target == this.caster)
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
            action.GiveToPlayer(this.caster, target,this, strength);
        }
    }
    public bool Activate(ShipDefines.PartEvent evt)
    {
        if (evt == data.function && CanBeActivated())
        {
            TourneyController.main.Inspect($"{caster} activates ability {data.InternalName}");
            float fuel = caster.abilities.fuel.GetValue();
            float fCost = GetFuelCost();

            float strength = 1;
            if (fCost > 0 && fuel < fCost)
            {
                if (data.overflow == ItemDefines.OverflowBehavior.chanceToFail)
                {
                    if (fCost == 0)
                    {
                        strength = 1;
                    }
                    else
                    {
                        float castChance = caster.abilities.fuel.GetValue() / fCost;
                        float luckCoefficient = ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus));
                        float ranVal = 1f - Mathf.Pow(1f - Random.value, luckCoefficient);
                        strength = ranVal > castChance ? 1 : 0;
                    }
                }
                else if (data.overflow == ItemDefines.OverflowBehavior.percentageStrength)
                {
                    strength = fCost == 0 ? 1 : (fuel / fCost);
                }
                else
                {
                    strength = fuel > fCost ? 1 : 0;
                }
            }
            if (strength>0)
            {
                ActivateOnRacer(strength);

                if (data.classification == ItemDefines.PartType.gadget)
                    caster.abilities.ListenToEvent(ShipDefines.PartEvent.OnGadgetActivate);
                else if (data.classification == ItemDefines.PartType.engine)
                    caster.abilities.ListenToEvent(ShipDefines.PartEvent.OnEngineActivate);
                else if (data.classification == ItemDefines.PartType.nitro)
                    caster.abilities.ListenToEvent(ShipDefines.PartEvent.OnNitroActivate);

                if (data.function == ShipDefines.PartEvent.OnActivated)
                    if (data.cooldown > 1 || data.fuelCost > 20)
                        caster.abilities.ListenToEvent(ShipDefines.PartEvent.OnBigAbilityActivate);
                    else
                        caster.abilities.ListenToEvent(ShipDefines.PartEvent.OnFastAbilityActivate);
            }
            caster.abilities.fuel.SubstractedValue(fCost);
            FireCooldown(1);
            return true;
        }
        return false;
    }

    public void FireCooldown(float mult )
    {
        Set(GetPartCooldown() * mult);
    }
    public void Use(float val = 1)
    {
        useCount += Mathf.CeilToInt(val);
    }
    public void RefreshUses(float val)
    {

        useCount =Mathf.Max(0, useCount-Mathf.CeilToInt(val));
    }
    public override string ToString()
    {
        return data.InternalName + " AbilityData";
    }
    public void RegisterGrantedSpeed(float amt)
    {
        grantedSpeed += amt;
    }
    public void RegisterGrantedFuel(float amt)
    {
        grantedFuel += amt;
    }
}