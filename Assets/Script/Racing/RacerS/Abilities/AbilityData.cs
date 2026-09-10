using System;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public delegate void AbilityFunction(Racer caster, Racer target, Ability source, float mult);
    public class AbilityListener
    {
        public RaceDefines.AbilityTarget effectSource, effectTarget;
        public AbilityFunction d;
    }

    public string InternalName;
    public int maxUses = 0;
    public float cooldown = 0;
    public float fuelCost = 0;
    public AiRacer.AiSignal aiSignal;
    public ItemDefines.OverflowBehavior overflow;
    public ItemDefines.PartType classification;
    public ShipDefines.PartEvent function;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public AbilityListener[] actions;

    public static AbilityData NpcWheel(int level, float rnval)
    {
        var action = new ConditionalPartAltetration()
        {
            behavior = ShipDefines.AlterationType.Addition,
            stat = ShipDefines.StatType.BaseSpeed,
            value = Mathf.Max(DifficultyDefines.enemyMinSpeed, DifficultyDefines.enemyBaseSpeed - DifficultyDefines.enemyWheelSpeed + DifficultyDefines.enemyWheelSpeed * (level + 1) * rnval)
        };
        return new AbilityData()
        {
            InternalName = "npc_wheel",
            function = ShipDefines.PartEvent.OnRaceStart,
            actions = new AbilityListener[]
                {
                    new AbilityListener()
                {
                d= (caster, target, source, mult) =>
                {
            action.GiveToPlayer(caster, target, source, mult);
        }
    }
    }
        };
    }
    public static AbilityData NpcEngine(int level, float rnval, float gasUse)
    {
        var action = new ConditionalPartAltetration()
        {
            behavior = ShipDefines.AlterationType.Addition,
            stat = ShipDefines.StatType.BoostSpeed,
            value = DifficultyDefines.enemyEngineSpeed * level * rnval
        };
        return new AbilityData()
        {
            InternalName = "npc_engine",
            function = ShipDefines.PartEvent.OnTimePass,
            cooldown = DifficultyDefines.enemyEngineCooldown - rnval * DifficultyDefines.enemyEngineDelta,
            fuelCost = gasUse * 10,
            actions = new AbilityListener[]
                {
                    new AbilityListener()
                {
                d= (caster, target, source, mult) =>
                {
                    action.GiveToPlayer(caster, target, source, mult);
                }
            }
            }
        };
    }
    public string GetAbilityDescription()
    {
        string output = $"{LanguageController.main.Translate("Abilities", "function_" + function)}: ";

        string effects = "";
        foreach (var a in actions)
        {
            string label = "";

            string numValue = Mathf.Abs(a.value).ToString();
            if (a.behavior == ShipDefines.AlterationType.Multiply)
            {
                numValue = Mathf.Abs(a.value * 100) + "% ";
                if (numValue[0] == '+')
                    numValue = "x" + label.Substring(1);
                else
                    numValue = "x" + label;
            }
            if (a.scale == ShipDefines.ScaleType.Constant)
            {
                label += LanguageController.main.Translate("Modifiers", a.value < 0 ? "Lose Effect" : "Gain Effect")
                   .Replace("%value%", numValue)
                   .Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + a.effectSource))
                   .Replace("%scale%", LanguageController.main.Translate("Modifiers", "scale_" + a.scale));
            }
            else
            {
                label += LanguageController.main.Translate("Modifiers", ((a.scale == ShipDefines.ScaleType.Lucky || a.scale == ShipDefines.ScaleType.Random) ? "Chance Scale " : "Stat Scale ") + (a.value > 0 ? "Pos" : "Neg"))
                    .Replace("%value%", numValue)
                    .Replace("%source%", LanguageController.main.Translate("Abilities", "source_" + a.effectSource))
                    .Replace("%scale%", LanguageController.main.Translate("Modifiers", "scale_" + a.scale));
            }
            label += LanguageController.main.Translate("Abilities", "effect_" + a.stat);
            label = LanguageController.main.Translate("Abilities", "target_" + a.effectTarget).Replace("%effect%", label);
            if (a.condition != ShipDefines.PartCondition.Always)
            {
                if (a.condition == ShipDefines.PartCondition.RelativeToRival)
                {
                    label = LanguageController.main.Translate("Abilities", a.conditionCheck < 0 ? "condition_BehindRival" : "condition_AheadOfRival" + function) + ": " + label;
                }
                else
                {
                    label = LanguageController.main.Translate("Abilities", "condition_" + a.condition).Replace("%value%", a.conditionCheck.ToString("F1")) + ": " + label;
                }
            }

            if (effects.Length > 0) effects += ", ";
            effects += label;
        }

        string costs = ". ";
        if (fuelCost > 0)
        {
            costs += LanguageController.main.Translate("Abilities", "Ability Cost").Replace("%value%", fuelCost.ToString());
        }
        if (cooldown > 0)
        {
            if (costs.Length > 0) costs += " ";
            costs += LanguageController.main.Translate("Abilities", "Ability Cooldown").Replace("%value%", cooldown.ToString());
        }
        if (maxUses > 0)
        {
            if (costs.Length > 0) costs += " ";
            costs += LanguageController.main.Translate("Abilities", "Ability Uses").Replace("%value%", maxUses.ToString());
        }


        return output + effects + costs;

    }
}