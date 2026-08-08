using System;
using UnityEngine;

[Serializable]
public class PartAbility
{
    public string InternalName;
    public int maxUses = 0;
    public float cooldown = 0;
    public float fuelCost = 0;
    public ItemDefines.PartType classification;
    public ShipDefines.PartEvent function;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public ConditionalPartAltetration[] actions;

    public static PartAbility NpcWheel(int level, float rnval)
    {
        return new PartAbility()
        {
            InternalName = "npc_wheel",
            function = ShipDefines.PartEvent.OnRaceStart,
            actions = new ConditionalPartAltetration[]{  new ConditionalPartAltetration()
            {
                behavior = ShipDefines.AlterationType.Addition,
                stat = ShipDefines.StatType.BaseSpeed,
                value =  DifficultyDefines.enemyBaseSpeed + DifficultyDefines.enemyWheelSpeed * level *rnval * (level == 0 ? .9f : 1f)
            }
            }
        };
    }
    public static PartAbility NpcEngine(int level, float rnval)
    {
        return new PartAbility()
        {
            InternalName = "npc_engine",
            function = ShipDefines.PartEvent.OnTimePass,
            cooldown = DifficultyDefines.enemyEngineCooldown - rnval * DifficultyDefines.enemyEngineDelta,
            fuelCost = 10,
            actions = new ConditionalPartAltetration[]{  new ConditionalPartAltetration()
            {
                behavior = ShipDefines.AlterationType.Addition,
                stat = ShipDefines.StatType.BoostSpeed,
                value =  DifficultyDefines.enemyEngineSpeed * (level+1) *rnval
            }
            }
        };
    }
    public string GetAbilityDescription()
    {
        string output = $"{function}: ";

        string effects = "";
        foreach (ConditionalPartAltetration a in actions)
        {


            string label = "";
            if (a.condition != ShipDefines.PartCondition.Always)
            {
                label +=$"if {a.condition} {a.conditionCheck}: ";
            }

            if (a.scale == ShipDefines.ScaleType.Constant)
            {
                label += a.value + " ";
            }
            else
            {
                label += $"{MathF.Round(a.value * 100)}% of {a.effectSource} {a.scale} as ";
            }
            if (a.behavior == ShipDefines.AlterationType.Multiply)
            {
                if (effects[0] == '+')
                    label = "x" + label.Substring(1);
                else
                    label = "x" + label;
            }
            label += a.stat;
            if (a.effectTarget != RaceDefines.AbilityTarget.Self)
            {
                label = a.effectTarget + " gets " + label;
            }

            if (effects.Length > 0) effects += ", ";
            effects += label;
        }

        string costs = ". ";
        if (fuelCost > 0)
        {
            costs += "Uses " + Mathf.Round(fuelCost) + " Fuel. ";
        }
        if (cooldown > 0)
        {
            costs += Mathf.Round(cooldown) + " Cooldown.";
        }

        return output + effects + costs;

    }
}