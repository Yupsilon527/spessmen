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
    public float abilityPower = 0;
    public PlayerStatsAlteration action;
}