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

        boost_speed_bonus = 4,
        boost_speed_percent = 5,

        tank_capacity = 6,

        incoming_speed_total = 7,
        incoming_speed_wheels = 8,
        incoming_speed_engines = 9,
        incoming_speed_nitro = 30,

        incoming_base_speed_percentage = 10,
        incoming_boost_speed_percentage = 11,
        incoming_gas_percentage = 12,
        engine_cooldown = 13,
        ability_cooldown = 14,
        gadget_cooldown = 28,
        nitro_cooldown = 29,

        ability_power = 22,
        cooldown_total = 23,

        engine_fuel_consumption = 15,
        gadget_fuel_consumtion = 16,
        nitro_fuel_consumtion = 26,
        active_fuel_consumtion = 27,
        fuel_consumption_total = 17,

        gold_income = 18,
        gold_bonus = 47,
        gold_interest = 19,

        opponent_speed = 20,
        rival_speed = 21,

        shop_resets = 24,
        luck_bonus = 25,

        effect_resistance = 48,
       speed_resistance = 49,

       engine_prices = 31,
       gadget_prices = 32,
       nitro_prices = 33,
       trinket_prices = 34,
       wheel_prices = 35,
       active_prices = 36,
       shop_prices = 37,

       engine_weight = 38,
       gadget_weight = 39,
       nitro_weight = 40,
       trinket_weight = 41,
       wheel_weight = 42,
       active_weight = 43,
       item_rarity = 44,
       
        expansion_rarity = 45,
        expansion_prices = 46,

        total = 50,
    };
    public enum State   //TODO
    {
        Stunned = 0,
        CanOvergas = 1,
        AbilityImmune = 2,
        Total = 3,
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

            case Property.incoming_speed_total:
            case Property.incoming_speed_wheels:
            case Property.incoming_speed_engines:
            case Property.incoming_speed_nitro:
            case Property.gadget_cooldown:
            case Property.nitro_cooldown:
            case Property.ability_power:
            case Property.cooldown_total:
            case Property.engine_fuel_consumption:
            case Property.gadget_fuel_consumtion:
            case Property.nitro_fuel_consumtion:
            case Property.active_fuel_consumtion:
            case Property.fuel_consumption_total:
            case Property.effect_resistance:
            case Property.speed_resistance:

            case Property.engine_prices:
            case Property.gadget_prices:
            case Property.nitro_prices:
            case Property.trinket_prices:
            case Property.wheel_prices:
            case Property.active_prices:
            case Property.shop_prices:
            case Property.expansion_prices:

            case Property.engine_weight:
            case Property.gadget_weight:
            case Property.nitro_weight:
            case Property.trinket_weight:
            case Property.wheel_weight:
            case Property.active_weight:
            case Property.item_rarity:
            case Property.expansion_rarity:
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
            return $"{(value > 0 ? "+" : "")}{Mathf.Round(value * 100)}%";
            else
                return $"{(value > 0 ? "+" : "")}{Mathf.RoundToInt(value * 10) / 10f}";

        }
    }
    [Serializable]
    public class RelativeStatData   //get property relative to stat
    {
        public ShipDefines.ScaleType baseStat;
        public bool reverse = false;
        public Property Property;
        public float translation;

        public float GetValueForRacer(Racer racer)
        {
            return ShipDefines.GetScale(racer, baseStat, reverse);
        }

        public string ValueToString()
        {
            if (IsPropertyMultiplicative(Property))
            {
                return $"+1% {Property} for every {Mathf.CeilToInt(1 / translation)* .01f} {baseStat}";
            }
            return $"+1 {Property} for every {Mathf.CeilToInt(1 / translation)} {baseStat}";
        }
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