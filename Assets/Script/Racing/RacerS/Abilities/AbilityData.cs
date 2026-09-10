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
}