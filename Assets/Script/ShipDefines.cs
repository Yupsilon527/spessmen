
public static class ShipDefines 
{
    public static int shipSize = 10;
    public enum PartFunction
    {
        OnRaceStart = 0,
        OnTimed = 1,
        OnActivated = 2,
    }
    public enum PartAction
    {
        GiveBaseSpeed = 0,
        GiveBoostSpeed = 1,
        GiveFuel = 2,
    }
}

public class PartAbility
{
    public float cooldown = 0;
    public float fuelCost = 0;
    public ShipDefines.PartFunction function;
    public ShipDefines.PartAction action;
    public float abilityPower = 0;
}