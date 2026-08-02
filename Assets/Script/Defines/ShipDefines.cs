
using System.Linq;
using UnityEngine;

public static class ShipDefines 
{
    public static int shipSize = 10;
    public static float soundBarrierSpeed = 100;
    public static float gasBase = 100;
    public enum AlterationType
    {
        Addition,
        Multiply,
        Divide,
        Min,
        Max,
    }
    public enum StatType
    {
        BaseSpeed,
        BoostSpeed,
        FillGas,
        TotalSpeed,
        RefreshEngines,
        RefreshGadgets,
        RefreshCooldowns,
        Total,
    }
    public enum PartEvent
    {
        OnRaceStart = 0,
        OnTimePass = 1,
        OnActivated = 2,
        OnLapCompleted = 3,
        OnOtherOvertaken = 4,
        OnRivalOvertaken = 5,
        OnSoundBarrierBroken = 6,
        OnGadgetActivate = 7,
        OnEngineActivate = 8,
        OnNitroActivate = 9,
        OnFastAbilityActivate = 10,
        OnBigAbilityActivate = 10,
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

        Lucky = 13,
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

        Random,
        Lucky,

        NumEngines,
        NumWheels,
        NumNitros,
        NumGadgets,
        NumTrinkets,

        CarSlots,
        TotalSlots,

        FuelTotal,

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
            case ScaleType.FuelTotal:
                return racer.abilities.fuel.GetLimit();
            case ScaleType.NumEngines:
                return DataItemPlayer.main.ship.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.engine ? 1 : 0);
            case ScaleType.NumWheels:
                return DataItemPlayer.main.ship.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.wheel ? 1 : 0);
            case ScaleType.NumNitros:
                return DataItemPlayer.main.ship.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.nitro ? 1 : 0);
            case ScaleType.NumTrinkets:
                return DataItemPlayer.main.ship.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.decal ? 1 : 0);
            case ScaleType.CarSlots:
                return DataItemPlayer.main.ship.CountTilesEmpty();
            case ScaleType.TotalSlots:
                return DataItemPlayer.main.ship.CountTilesTotal();
            case ScaleType.Random:
                return Random.value ;
            case ScaleType.Lucky:
                return ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus)) ;
            default:
                return 1;
        }
    }
    public static bool RacerMeetsCondition(Racer racer, PartCondition condition, float conditionCheck)
    {
        switch (condition)
        {
            case PartCondition.Random:
                return Random.value < conditionCheck;
            case PartCondition.Lucky:
                return ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus)) < conditionCheck;
            case PartCondition.SpeedBelow:
                return racer.stats.realSpeed < conditionCheck;
            case PartCondition.SpeedAbove:
                return racer.stats.realSpeed > conditionCheck;
            case PartCondition.PositionAbove:
                return TourneyController.main.ongoingRace.GetPositionForRacer(racer) < conditionCheck;
            case PartCondition.PositionBelow:
                return TourneyController.main.ongoingRace.GetPositionForRacer(racer) > conditionCheck;
            case PartCondition.RelativeToRival:
                var rival = racer.GetRival();
                if (rival == null) return false;
                if (conditionCheck > 0 && TourneyController.main.ongoingRace.GetPositionForRacer(racer) > TourneyController.main.ongoingRace.GetPositionForRacer(rival))
                    return true;
                else if (conditionCheck < 0 && TourneyController.main.ongoingRace.GetPositionForRacer(racer) < TourneyController.main.ongoingRace.GetPositionForRacer(rival))
                    return true;
                else if (conditionCheck == 0 && Mathf.Abs(TourneyController.main.ongoingRace.GetPositionForRacer(racer) - TourneyController.main.ongoingRace.GetPositionForRacer(rival)) <= 1)
                    return true;
                return false;
            case PartCondition.GasAbove:
                return racer.abilities.fuel.GetValue() > conditionCheck;
            case PartCondition.GasBelow:
                return racer.abilities.fuel.GetValue() < conditionCheck;
            case PartCondition.GasPercentAbove:
                return racer.abilities.fuel.GetPercentage() > conditionCheck;
            case PartCondition.GasPercentBelow:
                return racer.abilities.fuel.GetPercentage() < conditionCheck;
            case PartCondition.LapAbove:
                return racer.position.currentLap > conditionCheck;
            case PartCondition.LapBelow:
                return racer.position.currentLap < conditionCheck;
            default:
                return true;
        }
    }
}
