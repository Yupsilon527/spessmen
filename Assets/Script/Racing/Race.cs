using System.Collections.Generic;
using UnityEngine;

public class Race : Countdown
{
    public int raceID = 0;
    public float lapDistance = 210;
    public List<Racer> racers;
    public RaceDefines.RaceModifiers modifier = RaceDefines.RaceModifiers.Nothing;
    public Race(int id)
    {
        raceID = id;
        lapDistance = DifficultyDefines.lapDistanceBase + DifficultyDefines.lapDistanceAdd * raceID;
    }
    public int GetPositionForRacer(Racer racer)
    {
        return racers.IndexOf(racer);
    }
    public void UpdateLeaderboard()
    {
        racers.Sort((a, b) => b.position.distanceTraveled.CompareTo(a.position.distanceTraveled));
    }
    public float GetRivalDistance()
    {
        return GetRivalDistance(raceID, modifier == RaceDefines.RaceModifiers.LongerRace ? RaceDefines.raceLengthLong : RaceDefines.raceLength);
    }
    public static float GetRivalDistance(int level, float raceDuration = RaceDefines.raceLength)
    {
        var baseSpeed = DifficultyDefines.GetEnemyWheelSpeedAtLevel(level);

        int engineMin = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;
        int engineLevel = level - engineMin + 1;
        var engineSpeed = engineLevel > 0 ? (DifficultyDefines.enemyEngineSpeed * engineLevel) : 0;

        int numEngines = Mathf.FloorToInt(level / engineMin);
        float engineCooldown = DifficultyDefines.enemyEngineCooldown - DifficultyDefines.enemyEngineDelta;

        float gasCapacity = 100 + DifficultyDefines.enemyTankBonus * engineLevel;
        float gasUse = DifficultyDefines.enemyGasUse;
        int maxRevs = Mathf.CeilToInt(gasCapacity / gasUse);

        return GetRivalDistance(baseSpeed, numEngines, engineSpeed, engineCooldown, raceDuration, maxRevs);
    }
    public static float GetRivalDistance(float baseSpeed, float engineCount, float boostSpeed, float engineCD, float raceTime, int maxEngineUses)
    {
        if (engineCount <= 0)
            return baseSpeed * raceTime;
        float interval = engineCD / engineCount;
        int n = Mathf.Min(Mathf.FloorToInt(raceTime / interval) + 1, maxEngineUses);
        return baseSpeed * raceTime
             + boostSpeed * (n * raceTime - interval * n * (n - 1) / 2f);
    }
}