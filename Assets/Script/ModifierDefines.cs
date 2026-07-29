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
        base_speed = 0,
        base_speed_percent = 1,
        bonus_speed = 2,
        total_speed_percent = 3,

        boost_speed = 4,
        boost_speed_percent = 5,

        tank_capacity = 6,

        incoming_speed_total = 7,
        incoming_speed_wheels = 8,
        incoming_speed_engines = 9,

        incoming_base_speed_percentage = 10,
        incoming_boost_speed_percentage = 11,
        incoming_gas_percentage = 12,
        engine_cooldown = 13,
        ability_cooldown = 14,

        ability_power = 22,

        engine_fuel_consumption=15,
        ability_fuel_consumtion =16,
        fuel_consumption_total = 17,

        gold_income = 18,
        gold_interest = 19,

        opponent_speed = 20,
        rival_speed = 21,

        total = 23,
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
            case Property.base_speed_percent:
            case Property.total_speed_percent:
            case Property.boost_speed_percent:
            case Property.incoming_base_speed_percentage:
            case Property.incoming_boost_speed_percentage:
            case Property.incoming_gas_percentage:
            case Property.engine_cooldown:
            case Property.ability_cooldown:
            case Property.gold_income:
            case Property.gold_interest:
            case Property.opponent_speed:
            case Property.rival_speed:
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
                case PlayerStatsAlteration.StatType.BaseSpeed:
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