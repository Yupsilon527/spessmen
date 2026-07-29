
public static class ShipDefines 
{
    public static int shipSize = 10;
    public enum PartFunction
    {
        OnRaceStart = 0,
        OnTimed = 1,
        OnActivated = 2,
        OnLapCompleted = 3,
        OnOtherOvertaken = 4,
        OnSoundBarrierBroken = 5,
    }
    public enum PartCondition
    {
        SpeedBelow = 0,
        SpeedAbove = 1,

        PositionBelow = 2,
        PesitionAbove =3,

        RelativeToRival=4,

        GasAbove=5,
        GasBelow=6,
    }
}
