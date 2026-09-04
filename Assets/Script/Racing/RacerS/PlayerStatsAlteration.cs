using System;
using UnityEngine;
using static ShipDefines;

[Serializable]
public class PlayerStatsAlteration
{
    public AlterationType behavior = AlterationType.Addition;
    public StatType stat = StatType.BaseSpeed;
    public ScaleType scale = ScaleType.Constant;
    public float value = 0;
    public bool reverse = false;
    public ModifierDefines.Property GetRelatedProperty()
    {
        switch (stat)
        {
            case StatType.BaseSpeed:
                return ModifierDefines.Property.incoming_base_speed_percentage;
            case StatType.BoostSpeed:
                return ModifierDefines.Property.incoming_boost_speed_percentage;
            case StatType.FillGas:
                return ModifierDefines.Property.incoming_gas_percentage;
        }
        return ModifierDefines.Property.total;
    }
    public float GetEffectiveChange(Racer player, float mult, bool onSelf)
    {
        float output = value * GetScale( player, scale, reverse) * mult;
        if (onSelf && GetRelatedProperty() < ModifierDefines.Property.total)
            return output * player.GetPropertyMultiplicative(GetRelatedProperty());
        return output;
    }
    public void ApplyToStat(ref float stat, float value)
    {
        switch (behavior)
        {
            case AlterationType.Addition:
                stat += value;
                break;
            case AlterationType.Multiply:
                stat *= Mathf.Abs(value);
                break;
            case AlterationType.Divide:
                stat /= Mathf.Abs(value);
                break;
            case AlterationType.Min:
                stat = Mathf.Min(stat, value);
                break;
            case AlterationType.Max:
                stat = Mathf.Max(stat, value);
                break;

        }
    }
    public virtual bool CanAffectRacer(Racer target)
    {
        return target!=null;
    }
    public virtual void GiveToPlayer(Racer caster, Racer target, Ability source, float mult)
    {
        bool self = caster == target;
        float oldSpeed = target.stats.realSpeed;
        switch (stat)
        {
            case StatType.BaseSpeed:
                ApplyToStat(ref target.stats.baseSpeed, GetEffectiveChange(caster, mult, self));
                target.stats.SetDirty();
                source.RegisterGrantedSpeed(target.stats.realSpeed - oldSpeed); 
                break;
            case StatType.BoostSpeed:
                ApplyToStat(ref target.stats.boosterSpeed, GetEffectiveChange(caster, mult, self));
                target.stats.SetDirty();
                source.RegisterGrantedSpeed(target.stats.realSpeed - oldSpeed);
                break;
            case StatType.TotalSpeed:
                ApplyToStat(ref target.stats.alteredSpeed, GetEffectiveChange(caster, mult, self));
                target.stats.SetDirty();
                source.RegisterGrantedSpeed(target.stats.realSpeed - oldSpeed);
                break;
            case StatType.FillGas:
                float gasGiven = GetEffectiveChange(caster, mult, self);
                target.abilities.fuel.GiveValue(gasGiven);
                source.RegisterGrantedFuel(gasGiven);
                break;
            case StatType.RefundGasCost:
                float gasRefunded = GetEffectiveChange(caster, mult, self) * source.GetFuelCost();
                target.abilities.fuel.GiveValue(gasRefunded);
                source.RegisterGrantedFuel(gasRefunded);
                break;
            case StatType.GrantUse:
                source.RefreshUses(GetEffectiveChange(caster, mult, self));
                break;
            case StatType.RefreshCooldowns:
            case StatType.RefreshNitros:
            case StatType.RefreshEngines:
            case StatType.RefreshGadgets:
            case StatType.RefreshSelf:
                float CD = GetEffectiveChange(caster, mult, self);

                var valid = stat == StatType.RefreshSelf ? new Ability[] { source } :
                    stat == StatType.RefreshCooldowns ? target.abilities.GetAbilities() : 
                    target.abilities.GetAbilityByType(stat == StatType.RefreshNitros ? ItemDefines.PartType.nitro : 
                    stat == StatType.RefreshGadgets ? ItemDefines.PartType.gadget : ItemDefines.PartType.engine);

                foreach (var ab in valid)
                {
                    if (CD> 0)
                    ab.Shorten(CD);
                    else
                    ab.Extend(CD);
                }
                break;
        }
    }
}


[Serializable]
public class ConditionalPartAltetration : PlayerStatsAlteration
{
    public RaceDefines.AbilityTarget effectSource, effectTarget;
    public PartCondition condition;
    public float conditionCheck;

    public override bool CanAffectRacer(Racer target)
    {
        return base.CanAffectRacer(target) && RacerMeetsCondition(target, condition, conditionCheck);
    }

}