using System;
using UnityEngine;
using static ShipDefines;

[Serializable]
public class RacerStatsTable : RacerComponent
{
    public float realSpeed;
    public float gasTotal;
    public float baseSpeed;
    public float boosterSpeed;
    bool requiresUpdate = false;
    bool brokenSoundBarrier = false;

    public RacerStatsTable(Racer racer) : base(racer)
    {
    }
    public override void HandleRacePhase(RaceDefines.RacePhase phase)
    {
        base.HandleRacePhase(phase);
        switch (phase)
        {
            case RaceDefines.RacePhase.RaceBegin:
                realSpeed = 0;
                baseSpeed = 0;
                boosterSpeed = 0;
                gasTotal = gasBase;
                brokenSoundBarrier = false;
                break;
            case RaceDefines.RacePhase.RaceTick:
                if (requiresUpdate) UpdateRealSpeed();
                break;
        }
    }
    public void SetDirty()
    {
        requiresUpdate = true;
    }
    public void UpdateRealSpeed()
    {
        float bSpeed = baseSpeed;
        bSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.base_speed);
        bSpeed *= racer.GetPropertyMultiplicative(ModifierDefines.Property.base_speed_percent);
        bSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.bonus_speed) * racer.GetPropertyMultiplicative(ModifierDefines.Property.total_speed_percent);

        float tSpeed = boosterSpeed;
        tSpeed += racer.GetPropertyAdditive(ModifierDefines.Property.boost_speed_bonus);
        tSpeed *= racer.GetPropertyMultiplicative(ModifierDefines.Property.boost_speed_percent);

        realSpeed = bSpeed + tSpeed;

        var playerRacer = TourneyController.main.GetPlayerRacer();
        if (playerRacer!=racer)
        {
            realSpeed *= playerRacer.GetPropertyMultiplicative(ModifierDefines.Property.opponent_speed);
            if (playerRacer.GetRival()==racer)
            {
                realSpeed *= playerRacer.GetPropertyMultiplicative(ModifierDefines.Property.rival_speed);
            }
        }


        if (!brokenSoundBarrier && realSpeed > soundBarrierSpeed)
        {
            racer.abilities.ListenToEvent(PartEvent.OnSoundBarrierBroken);
            brokenSoundBarrier = true;
        }


        requiresUpdate = false;
    }
    public void UpdateGasTotal()
    {
        gasTotal = gasBase + racer.GetPropertyAdditive(ModifierDefines.Property.tank_capacity);
    }

    public RacerStatsTable Clone()
    {
        return MemberwiseClone() as RacerStatsTable;
    }
}

[Serializable]
public class PlayerStatsAlteration
{
    public enum AlterationType
    {
        Addition,
        Multiply,
        Divide,
        Min,
        Max,
        Random,
    }
    public enum StatType
    {
        BaseSpeed,
        BoostSpeed,
        FillGas,
        Total,
    }
    public AlterationType behavior = AlterationType.Addition;
    public StatType stat = StatType.BaseSpeed;
    public ScaleType scale = ScaleType.Constant;
    public float value = 0;
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
    public float GetEffectiveChange(Racer player, float mult)
    {
        value *= GetScale( player, scale) * mult;
        if (GetRelatedProperty() < ModifierDefines.Property.total)
            return value * player.GetPropertyMultiplicative(GetRelatedProperty());
        return value;
    }
    public void GiveToPlayer(Racer player, float mult)
    {
        switch (stat)
        {
            case StatType.BaseSpeed:
                ApplyToStat(ref player.stats.baseSpeed, GetEffectiveChange(player, mult));
                player.stats.SetDirty();
                break;
            case StatType.BoostSpeed:
                ApplyToStat(ref player.stats.boosterSpeed, GetEffectiveChange(player, mult));
                player.stats.SetDirty();
                break;
            case StatType.FillGas:
                player.abilities.fuel.GiveValue(GetEffectiveChange(player, mult));
                break;
        }
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
            case AlterationType.Random:
                stat += UnityEngine.Random.value * value;
                break;

        }
    }
}