
using System.Linq;
using UnityEngine;

public static class ShipDefines
{
    public static int shipSize = 10;
    public const float soundBarrierSpeed = 100;
    public const float gasBase = 100;
    public static Vector2Int[] deltaPos = new Vector2Int[]{
        Vector2Int.zero,
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
        };
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
        RefreshNitros,
        RefreshCooldowns,
        RefreshSelf,
        RefundGasCost,
        GrantUse,
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
        OnBigAbilityActivate = 11,
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

    public static float GetScale(Racer racer, ScaleType scale,bool reverse)
    {
        switch (scale)
        {
            case ScaleType.BaseSpeed:
                return racer.stats.baseSpeed * (reverse ? -1 : 1);
            case ScaleType.BoostSpeed:
                return racer.stats.boosterSpeed * (reverse ? -1 : 1);
            case ScaleType.TotalSpeed:
                return racer.stats.realSpeed * (reverse ? -1 : 1);
            case ScaleType.DistanceTraveled:
                return racer.position.distanceTraveled * (reverse ? -1 : 1);
            case ScaleType.LapsCompleted:
                return racer.position.currentLap * (reverse ? -1 : 1);
            case ScaleType.CurrentPosition:
                int pos = TourneyController.main.ongoingRace.GetPositionForRacer(racer);
                return reverse ? (TourneyController.main.ongoingRace.racers.Count - pos) : pos;
            case ScaleType.CurrentRivalPosition:
                 pos = racer.GetRival() == null ? 0: TourneyController.main.ongoingRace.GetPositionForRacer(racer.GetRival());
                return reverse ? (TourneyController.main.ongoingRace.racers.Count - pos) : pos;
            case ScaleType.RivalDistanceTraveled:
                return racer.GetRival()?.position.distanceTraveled ?? 0;
            case ScaleType.CurrentFuelValue:
                return reverse ? (racer.abilities.fuel.GetLimit() - racer.abilities.fuel.GetValue()) : racer.abilities.fuel.GetValue();
            case ScaleType.CurrentFuelPercent:
                return reverse ? (1- racer.abilities.fuel.GetPercentage()) : racer.abilities.fuel.GetPercentage();
            case ScaleType.FuelTotal:
                return racer.abilities.fuel.GetLimit();
            case ScaleType.NumEngines:
                return DataItemPlayer.main.car.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.engine ? 1 : 0);
            case ScaleType.NumWheels:
                return DataItemPlayer.main.car.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.wheel ? 1 : 0);
            case ScaleType.NumNitros:
                return DataItemPlayer.main.car.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.nitro ? 1 : 0);
            case ScaleType.NumTrinkets:
                return DataItemPlayer.main.car.parts.Sum(part => part.scriptable.partType == ItemDefines.PartType.decal ? 1 : 0);
            case ScaleType.CarSlots:
                return reverse ? (DataItemPlayer.main.car.CountTilesTotal() - DataItemPlayer.main.car.CountTilesEmpty()) : DataItemPlayer.main.car.CountTilesEmpty();
            case ScaleType.TotalSlots:
                return DataItemPlayer.main.car.CountTilesTotal();
            case ScaleType.Random:
                return reverse ? (1- Random.value) : Random.value;
            case ScaleType.Lucky:
                float luckCoefficient = ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus));
                float ranVal = 1f - Mathf.Pow(1f - Random.value, luckCoefficient);
                return reverse ? (1- ranVal) : ranVal;
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
                float luckCoefficient = ItemDefines.LuckNumber(DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.luck_bonus));
                float ranVal = 1f - Mathf.Pow(1f - Random.value, luckCoefficient);
                return ranVal < conditionCheck;
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
