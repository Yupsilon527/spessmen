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
       base_speed,
       base_speed_percent,
       bonus_speed,
       total_speed_percent,

       boost_speed,
       boost_speed_percent,

       tank_capacity,

       incoming_speed_total,
       incoming_speed_wheels,
       incoming_speed_engines,

       incoming_gas_percentage,

       engine_cooldown,

       gold_income,
       gold_interest,
       
       opponent_speed,
       rival_speed,

       total,

    };
    public enum State   //TODO
    {
        Stunned = 0,
        CanOvergas = 1,
        Total = 2,
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
    #region States
    [Serializable]
    public class StateData
    {
        public State State;
        public float priority;
        public StateData() { }

        public StateData(State state, float priority)
        {
            State = state;
            this.priority = priority;
        }
    }
    #endregion

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
    [Serializable]
    public class RelativeStatData   //get property relative to stat
    {
        public PlayerStatsAlteration.StatType baseStat;
        public Property Property;
        public float translation;

        public float GetValueForRacer(Racer player)
        {
            switch (baseStat)
            {
                case PlayerStatsAlteration.StatType.Attack:
                    return player.stats.Attack * translation;
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