using System;
using UnityEngine;

public static class ModifierDefines
{

    public enum Flag
    {
        Nothing = -6,
        Undispellable = 0,
        Positive = 1,
        Regeneration = 2,
        Negative = -1,
        Disable = -2,
        DamageOverTime = -3,
        SoftDisable = -4,
        HardDisable = -5,
    }
    public enum Priority
    {
        low = 0,
        normal = 1,
        high = 2
    }
    public enum Behavior
    {
        Nothing = -1,
        Multiple = 0,
        Replace = 1,
        Unique = 2,
        IncreaseStacks = 3,
        IncreaseDuration = 4,
    }
    public enum Property
    {
        attack_bonus = 0,
        attack_bonus_percent = 1,
        special_bonus = 2,
        special_bonus_percent = 3,
        control_bonus = 4,
        control_bonus_percent = 5,
        attack_speed_bonus = 6,
        attack_speed_bonus_percent = 7,
        vampirism_bonus = 8,
        vampirism_bonus_percent = 9,
        cannibalism_bonus = 10,
        cannibalism_bonus_percent = 11,
        attack_cooldown_multiplier = 57,

        health_bonus = 12,
        health_bonus_percent = 13,
        armor_bonus = 14,
        armor_bonus_percent = 15,

        health_regen_bonus = 16,
        health_regen_percentage = 17,
        health_regen_percentage_total = 18,

        bonus_healing_from_pickups = 19,
        healing_start_wave = 20,

        move_speed = 21,
        move_speed_mult = 22,
        move_speed_override = 23,

        projectile_count = 24,
        projectile_spread = 25,
        projectile_speed = 26,
        projectile_duration = 27,
        projectile_scale = 28,
        projectile_piercing = 29,
        projectile_bounces = 30,
        explosion_radius = 31,
        explosion_damage = 32,

        incoming_damage = 33,
        outgoing_damage = 34,
        outgoing_damage_percent = 56,
        incoming_healing = 35,
        incoming_healing_absolute = 36,

        incoming_melee = 37,
        incoming_range = 38,
        incoming_area = 39,
        outgoing_melee = 40,
        outgoing_range = 41,
        outgoing_area = 42,

        money_income = 43,
        money_interest = 44,
        money_income_percent = 45,
        pickup_range = 46,
        shop_resets = 47,
        item_price = 48,

        luck_bonus = 49,
        luck_bonus_percent = 50,
        experience_gain_percent = 51,

        crit_chance = 52,
        crit_damage = 53,

        enemies_spawned = 54,
        enemies_health = 55,
        enemies_speed = 56,
        enemies_damage = 57,

        curse_enemy_chance = 58,
        curse_items_chance = 59,

        powerup_projectile_multiplier = 60,
        powerup_projspeed_multiplier = 61,
        powerup_attack_speed_multiplier = 62,
        powerup_attackdamage_multiplier = 63,
        powerup_shot_multiplier = 64,

        element_modifier = 65,
        element_damage = 66,

        Total = 67,
    };
    public enum State   //TODO
    {
        Nothing = 0,
        Total = 1,
    }
    public static bool IsPropertyMultiplicative(Property Property)
    {
        switch (Property)
        {
            case Property.vampirism_bonus:
            case Property.health_regen_percentage:
            case Property.health_regen_percentage_total:
            case Property.move_speed_mult:
            case Property.projectile_speed:
            case Property.projectile_scale:
            case Property.incoming_damage:
            case Property.outgoing_damage:
            case Property.incoming_healing:
            case Property.incoming_healing_absolute:
            case Property.incoming_melee:
            case Property.incoming_range:
            case Property.incoming_area:
            case Property.outgoing_melee:
            case Property.outgoing_range:
            case Property.outgoing_area:
            case Property.pickup_range:
                return true;
            default:
                return false;
        }
    }

    #region Properties
    [Serializable]
    public class PropertyData
    {
        public Property Property;
        public float value;
        public float IncreasePerLevel;

        public string ValueToString(int level, bool inclBase, float levelScale)
        {
            float baseValue = (inclBase ? value : 0) + IncreasePerLevel * level * levelScale;
            return ValueToString(Property, baseValue);
        }
        public static string ValueToString(Property property, float value)
        {
            if (IsPropertyMultiplicative(property))
                return $"{(value > 0 ? "+" : "")}{Mathf.RoundToInt(value * 10) / 10f}";
            else
                return $"{(value > 0 ? "+" : "")}{Mathf.Round(value * 100)}%";

        }
    }
    #endregion
    #region Relative
    [Serializable]
    public class RelativeStatData
    {
        public PlayerStatsAlteration.StatType baseStat;
        public Property Property;
        public float translation;

        public float GetValueForPlayer(Racer player)
        {
            switch (baseStat)
            {
                case PlayerStatsAlteration.StatType.Attack:
                    return player.stats.Attack * translation;
                case PlayerStatsAlteration.StatType.Special:
                    return player.stats.Special * translation;
                case PlayerStatsAlteration.StatType.Control:
                    return player.stats.CrowdControl * translation;
                case PlayerStatsAlteration.StatType.AttackSpeed:
                    return player.stats.AttackSpeed * translation;
                case PlayerStatsAlteration.StatType.Vampirism:
                    return player.stats.Vampirism * translation;
                case PlayerStatsAlteration.StatType.Cannibalism:
                    return player.stats.Cannibalism * translation;
                case PlayerStatsAlteration.StatType.Health:
                    return player.stats.Health * translation;
                case PlayerStatsAlteration.StatType.Armor:
                    return player.stats.Armor * translation;
                case PlayerStatsAlteration.StatType.Regeneration:
                    return player.stats.Regeneration * translation;
                case PlayerStatsAlteration.StatType.Nutrition:
                    return player.stats.Nutrition * translation;
                case PlayerStatsAlteration.StatType.MovementSpeed:
                    return player.stats.MovementSpeed * translation;
                case PlayerStatsAlteration.StatType.PickupRange:
                    return player.stats.PickupRange * translation;
                case PlayerStatsAlteration.StatType.Luck:
                    return player.stats.Luck * translation;
                default:
                    return 0;
            }
        }

        /*public string ValueToString(int level, bool inclBase, float levelScale)
        {
            float baseValue = (inclBase ? value : 0) + IncreasePerLevel * level * levelScale;
            return ValueToString(Property, baseValue);
        }
        public static string ValueToString(Property property, float value)
        {
            if (IsPropertyMultiplicative(property))
                return $"{(value > 0 ? "+" : "")}{Mathf.RoundToInt(value * 10) / 10f}";
            else
                return $"{(value > 0 ? "+" : "")}{Mathf.Round(value * 100)}%";

        }*/
    }
    #endregion
    public static string GetPropertyTable(PropertyData[] properties, int level, float levelValue)
    {
        string desc = "";
        foreach (var prop in properties)
        {
            if (!string.IsNullOrEmpty(desc)) desc += "<br>";
            desc += $"{prop.ValueToString(level, true, levelValue)} {"prop_" + prop.Property.ToString()}";
        }
        return desc;
    }
}