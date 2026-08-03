using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class TourneyController : Initializable
{
    public static TourneyController main;
    Dictionary<Racer, float> leaderboard = new();


    public Race ongoingRace;
    public enum TourneyPhase
    {
        beforeRace,
        racing,
        afterRace,
    }
    public TourneyPhase currentPhase;
    public void ChangePhase(TourneyPhase nPhase)
    {
        Inspect("Change Phase " + nPhase);
        switch (nPhase)
        {
            case TourneyPhase.beforeRace:
                if (leaderboard == null || leaderboard.Count == 0)
                    InitRacers();
                if ( GetCurrentRaceIndex() % (RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons) == 0)
                {
                    foreach (var racer in leaderboard.Keys.ToArray())
                        leaderboard[racer] = 0;
                }
                ongoingRace = new Race()
                {
                    raceID = ongoingRace == null ? 0 : ongoingRace.raceID + 1,
                    racers = leaderboard.Keys.Select(k => k).ToList(),
                };
                break;
            case TourneyPhase.racing:
                if (currentPhase == TourneyPhase.beforeRace || ongoingRace == null || !ongoingRace.IsRunning())
                {
                    Inspect($"Start race {ongoingRace.raceID} with {ongoingRace.racers.Count} racers!");
                    foreach (var racer in ongoingRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceBegin);
                    }
                    debugSet = 0;
                    ongoingRace.Set(20);
                }
                break;
            case TourneyPhase.afterRace:
                if (currentPhase == TourneyPhase.racing)
                {
                    foreach (var racer in ongoingRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceEnd);
                    }
                    foreach (var racer in leaderboard.Keys.ToArray())
                    {
                        leaderboard[racer] += GetPointsForPosition(ongoingRace.GetPositionForRacer(racer));
                    }
                    HandlePlayerReward();
                    UpdateVariables();
                }
                break;
        }
        currentPhase = nPhase;
    }
    public int GetCurrentRaceIndex()
    {
        return ongoingRace?.raceID ?? 0;
    }
    public bool IsLastRaceInSeason()
    {
        int total = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;
        return GetCurrentRaceIndex() % total == total - 1;
    }
    public float GetPointsForPosition(int position)
    {
        return leaderboard.Count - position - 1;
    }
    public float GetScoreForRacer(Racer racer)
    {
        return leaderboard[racer];
    }
    public Racer[] GetLeaderboardSorted()
    {
        return leaderboard
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToArray();
    }
    protected override void Initialize()
    {
        main = this;
        base.Initialize();
    }
    public void InitRacers(int opponents = 5)
    {
        leaderboard.Add(new PlayerRacer(DataItemPlayer.main.car), 0);
        for (int i = 0; i < opponents; i++)
        {
            leaderboard.Add(new AiRacer(i + 1), 0);
        }
    }
    public Racer GetPlayerRacer()
    {
        return leaderboard.FirstOrDefault(r => r.Key.id == 0).Key;
    }
    public Racer GetPlayerRival()
    {
        if (ongoingRace == null || GetCurrentRaceIndex() == 0) return null;
        return leaderboard
            .Where(kvp => kvp.Key.id != 0)
            .Select(kvp => kvp.Key)
            .LastOrDefault();
    }
    private void FixedUpdate()
    {
        if (currentPhase == TourneyPhase.racing)
        {
            if (ongoingRace.IsRunning())
            {
                foreach (var racer in ongoingRace.racers)
                    racer.HandleRacePhase(RaceDefines.RacePhase.RaceTick);
                ongoingRace.UpdateLeaderboard();
                DebugRace();
            }
            else
                ChangePhase(TourneyPhase.afterRace);
        }
    }
    public bool CanPlayerProceed()
    {
        return !IsLastRaceInSeason() || GetLeaderboardSorted()[0].id == 0 ; 
    }
    void HandlePlayerReward()
    {
        float diffMult = Mathf.Pow(EconomyDefines.goldPerRaceIncrease, GetCurrentRaceIndex());

        if (ongoingRace.raceID % DifficultyDefines.eliteRaceInterval == DifficultyDefines.eliteRaceInterval-1)
            diffMult *= DifficultyDefines.eliteRaceMultiplier;

        DataItemPlayer.main.score.GiveChaos(ItemDefines.chaosPerRace * diffMult);

        int playerPos = ongoingRace.GetPositionForRacer(GetPlayerRacer());
        float interest = DataItemPlayer.main.econ.gold.GetValue()* EconomyDefines.constantGoldInterest + DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_interest) ;

        float outputGold = EconomyDefines.constantGoldForRace * diffMult
            + DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_bonus)
            + Mathf.Floor(EconomyDefines.constantGoldPerPosition * (ongoingRace.racers.Count * playerPos)) * diffMult * DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_income);

        float distanceGold = 0;
        if (playerPos==0)
        {
            distanceGold = Mathf.FloorToInt((ongoingRace.racers[0].position.distanceTraveled - ongoingRace.racers[1].position.distanceTraveled) * EconomyDefines.constantGoldPerDistance );
        }

        Inspect($"Give player {interest + outputGold + distanceGold} gold; {outputGold} base, {distanceGold} performance and {interest} interest");

        DataItemPlayer.main.econ.GiveGold(interest + outputGold + distanceGold);
    }

    void UpdateVariables()
    {
        var playerRacer = GetPlayerRacer();

        DataItemPlayer.main.scope.SetVariable("race_position_"+ongoingRace.raceID, ongoingRace.GetPositionForRacer(GetPlayerRacer()));
        DataItemPlayer.main.scope.SetVariable("race_distance_"+ongoingRace.raceID, playerRacer.position.distanceTraveled);
        DataItemPlayer.main.scope.SetVariable("race_topspeed_"+ongoingRace.raceID, playerRacer.stats.realSpeed);
    }

    float debugSet = 0;
    void DebugRace()
    {
        int x = 2;
        if (ongoingRace.GetTimeRemaining()<20- debugSet*x)
        {
            Inspect($"--- RACE UPDATE ({debugSet * x}) ---");
            int i = 0;
            foreach (var racer in ongoingRace.racers)
            {
                Inspect($"Racer {racer.id} is in position {i + 1} with {racer.position.distanceTraveled} distance going fast at {racer.stats.baseSpeed} mph! Fuel: {racer.abilities.fuel.GetValue()}/{racer.abilities.fuel.GetLimit()}");
                i++;
            }
            debugSet++;
        }
    }
}
public class Race : Countdown
{
    public int raceID = 0;
    public float lapDistance = 210;
    public List<Racer> racers;

    public int GetPositionForRacer(Racer racer)
    {
        return racers.IndexOf(racer);
    }
    public void UpdateLeaderboard()
    {
        racers.Sort((a, b) => b.position.distanceTraveled.CompareTo(a.position.distanceTraveled));
    }
}