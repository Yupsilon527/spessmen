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
    }
    public enum StatType
    {
        Attack,
        Special,
        Control,
        AttackSpeed,
        Vampirism,
        Cannibalism,
        Health,
        Armor,
        Regeneration,
        Nutrition,
        MovementSpeed,
        PickupRange,
        Luck,
    }
    public ModifierDefines.Property GetRelatedProperty()
    {
        switch (stat)
        {
            case StatType.Attack:
                return ModifierDefines.Property.attack_bonus_percent;
            case StatType.Special:
                return ModifierDefines.Property.special_bonus_percent;
            case StatType.Control:
                return ModifierDefines.Property.control_bonus_percent;
            case StatType.AttackSpeed:
                return ModifierDefines.Property.attack_bonus_percent;
            case StatType.Vampirism:
                return ModifierDefines.Property.vampirism_bonus_percent;
            case StatType.Cannibalism:
                return ModifierDefines.Property.cannibalism_bonus_percent;
            case StatType.Health:
                return ModifierDefines.Property.health_bonus_percent;
            case StatType.Armor:
                return ModifierDefines.Property.armor_bonus_percent;
            case StatType.Regeneration:
                return ModifierDefines.Property.health_regen_percentage;
            case StatType.Luck:
                return ModifierDefines.Property.luck_bonus_percent;
        }
        return ModifierDefines.Property.Total;
    }
    public float GetEffectiveChange(Racer player)
    {
        if (GetRelatedProperty() < ModifierDefines.Property.Total)
            return value * player.GetPropertyMultiplicative(GetRelatedProperty());
        return value;
    }
    public AlterationType behavior = AlterationType.Addition;
    public StatType stat = StatType.Attack;
    public float value = 0;
    public void GiveToPlayer(Racer player)
    {
        switch (stat)
        {
            case StatType.Attack:
                ApplyToStat(ref player.stats.Attack, GetEffectiveChange(player));
                break;
            case StatType.Special:
                ApplyToStat(ref player.stats.Special, GetEffectiveChange(player));
                break;
            case StatType.Control:
                ApplyToStat(ref player.stats.CrowdControl, GetEffectiveChange(player));
                break;
            case StatType.AttackSpeed:
                ApplyToStat(ref player.stats.AttackSpeed, GetEffectiveChange(player));
                break;
            case StatType.Vampirism:
                ApplyToStat(ref player.stats.Vampirism, GetEffectiveChange(player));
                break;
            case StatType.Cannibalism:
                ApplyToStat(ref player.stats.Cannibalism, GetEffectiveChange(player));
                break;
            case StatType.Health:
                ApplyToStat(ref player.stats.Health, GetEffectiveChange(player));
                break;
            case StatType.Armor:
                ApplyToStat(ref player.stats.Armor, GetEffectiveChange(player));
                break;
            case StatType.Regeneration:
                ApplyToStat(ref player.stats.Regeneration, GetEffectiveChange(player));
                break;
            case StatType.Nutrition:
                ApplyToStat(ref player.stats.Nutrition, GetEffectiveChange(player));
                break;
            case StatType.MovementSpeed:
                ApplyToStat(ref player.stats.MovementSpeed, GetEffectiveChange(player));
                break;
            case StatType.PickupRange:
                ApplyToStat(ref player.stats.PickupRange, GetEffectiveChange(player));
                break;
            case StatType.Luck:
                ApplyToStat(ref player.stats.Luck, GetEffectiveChange(player));
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