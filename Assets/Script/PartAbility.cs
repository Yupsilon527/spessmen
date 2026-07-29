using System;

[Serializable]
public class PartAbility
{
    public string InternalName;
    public float cooldown = 0;
    public float fuelCost = 0;
    public ShipDefines.PartEvent function;
    public ShipDefines.PartCondition condition;
    public float conditionCheck;
    public PlayerStatsAlteration action;

    public static PartAbility NpcWheel(int level)
    {
        float speed = 50;
        if (level == 0) 
            speed = 30;
        return new PartAbility()
        {
            InternalName = "npc_wheel",
            function = ShipDefines.PartEvent.OnRaceStart,
            action = new PlayerStatsAlteration()
            {
                behavior = PlayerStatsAlteration.AlterationType.Addition,
                stat = PlayerStatsAlteration.StatType.BaseSpeed,
                value = level * speed + UnityEngine.Random.value * speed

            }
        };
    }
}