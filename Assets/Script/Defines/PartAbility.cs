using System;
using UnityEngine;

[Serializable]
public class PartAbility
{
    public string InternalName;
    public float cooldown = 0;
    public float fuelCost = 0;
    public ItemDefines.PartType classification;
    public ShipDefines.PartEvent function;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public ConditionalPartAltetration[] actions;

    public static PartAbility NpcWheel(int level)
    {
        float speed = DifficultyDefines.enemyBaseSpeed * (1 + level);

        return new PartAbility()
        {
            InternalName = "npc_wheel",
            function = ShipDefines.PartEvent.OnRaceStart,
            actions = new ConditionalPartAltetration[]{  new ConditionalPartAltetration()
            {
                behavior = PlayerStatsAlteration.AlterationType.Addition,
                stat = PlayerStatsAlteration.StatType.BaseSpeed,
                value =  speed * (level + UnityEngine.Random.value )
            }
            }
        };
    }
    public static PartAbility NpcEngine(int level)
    {
        float speed = DifficultyDefines.enemyEngineSpeed * (1 + level);

        return new PartAbility()
        {
            InternalName = "npc_engine",
            function = ShipDefines.PartEvent.OnTimePass,
            cooldown = 2 + UnityEngine.Random.value,
            fuelCost = 10,
            actions = new ConditionalPartAltetration[]{  new ConditionalPartAltetration()
            {
                behavior = PlayerStatsAlteration.AlterationType.Addition,
                stat = PlayerStatsAlteration.StatType.BoostSpeed,
                value =  speed * (level + UnityEngine.Random.value )
            }
            }
        };
    }
}