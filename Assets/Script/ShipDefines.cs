
public static class ShipDefines 
{
    public static int shipSize = 10;
    public static float soundBarrierSpeed = 100;
    public static float gasBase = 100;
    public enum PartEvent
    {
        OnRaceStart = 0,
        OnTimePass = 1,
        OnActivated = 2,
        OnLapCompleted = 3,
        OnOtherOvertaken = 4,
        OnRivalOVertaken = 5,
        OnSoundBarrierBroken = 6,
    }
    public enum PartCondition
    {
        Always = 0,
        Random = 1,
        SpeedBelow = 2,
        SpeedAbove = 3,

        PositionBelow = 4,
        PositionAbove =5,

        RelativeToRival=6,

        GasAbove=7,
        GasBelow=8,

        GasPercentAbove=9,
        GasPercentBelow = 10,

        LapBelow = 11,
        LapAbove = 12,
    }
    public enum ScaleType
    {
        Constant,

        BaseSpeed,
        BoostSpeed,

        TotalSpeed,

        DistanceTraveled,

        LapsCompleted,
        CurrentPosition,

        CurrentRivalPosition,
        RivalDistanceTraveled,

        CurrentFuelValue,
        CurrentFuelPercent,

        Total,
    }

    public static float GetScale(Racer racer, ScaleType scale)
    {
        switch (scale)
        {
            case ScaleType.BaseSpeed:
                return racer.stats.baseSpeed;
            case ScaleType.BoostSpeed:
                return racer.stats.boosterSpeed;
            case ScaleType.TotalSpeed:
                return racer.stats.realSpeed;
            case ScaleType.DistanceTraveled:
                return racer.position.distanceTraveled;
            case ScaleType.LapsCompleted:
                return racer.position.currentLap;
            case ScaleType.CurrentPosition:
                return TourneyController.main.ongoingRace.GetPositionForRacer(racer) ;
            case ScaleType.CurrentRivalPosition:
                return racer.GetRival() == null ? 0: TourneyController.main.ongoingRace.GetPositionForRacer(racer.GetRival());
            case ScaleType.RivalDistanceTraveled:
                return racer.GetRival()?.position.distanceTraveled ?? 0;
            case ScaleType.CurrentFuelValue:
                return racer.abilities.fuel.GetValue();
            case ScaleType.CurrentFuelPercent:
                return racer.abilities.fuel.GetPercentage();
            default:
                return 1;
        }
    }
}
