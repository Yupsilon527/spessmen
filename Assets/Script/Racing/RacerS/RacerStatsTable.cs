using System;
using UnityEngine;

[Serializable]
public class RacerStatsTable
{
    public float realSpeed;
    public float gasTotal;
    public float baseSpeed;
    public float boosterSpeed;

    public void GiveBaseSpeed(float amt)
    {
        baseSpeed += amt;
    }
    public void GiveBoostSpeed(float amt)
    {
        boosterSpeed += amt;
    }
    public void UpdateRealSpeed()
    {
        realSpeed = baseSpeed + boosterSpeed;
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
        TotalSpeed,

        DistanceTraveled,

        LapsCompleted,
        CurrentPosition,

        CurrentRivalPosition,
        RivalDistanceTraveled,
    }
    public AlterationType behavior = AlterationType.Addition;
    public StatType stat = StatType.Attack;
    public float value = 0;
    public ModifierDefines.Property GetRelatedProperty()
    {
        switch (stat)
        {
            case StatType.Attack:
                return ModifierDefines.Property.attack_bonus_percent;
        }
        return ModifierDefines.Property.Total;
    }
    public float GetEffectiveChange(Racer player)
    {
        if (GetRelatedProperty() < ModifierDefines.Property.Total)
            return value * player.GetPropertyMultiplicative(GetRelatedProperty());
        return value;
    }
    public void GiveToPlayer(Racer player)
    {
        switch (stat)
        {
            case StatType.Attack:
                ApplyToStat(ref player.stats.Attack, GetEffectiveChange(player));
                break;
        }
    }
    public void ApplyToStat(ref float stats, float value)
    {
        switch (behavior)
        {
            case AlterationType.Addition:
                stats += value;
                break;
            case AlterationType.Multiply:
                stats *= Mathf.Abs(value);
                break;
            case AlterationType.Divide:
                stats /= Mathf.Abs(value);
                break;
            case AlterationType.Min:
                stats = Mathf.Min(stats, value);
                break;
            case AlterationType.Max:
                stats = Mathf.Max(stats, value);
                break;

        }
    }
}