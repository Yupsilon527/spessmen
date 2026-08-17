using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class TourneyController : Initializable
{
    public static TourneyController main;
    Dictionary<Racer, float> leaderboard = new();
    public EnvironmentScriptable tournamentEnvironment;
    public Countdown raceCountdown = new();

    public Race ongoingRace;
    public enum TourneyPhase
    {
        beforeRace,
        setup,
        racing,
        afterRace,
    }
    public TourneyPhase currentPhase;
    public void FreshStart()
    {
        leaderboard.Clear();
        ongoingRace = null;
        InitRacers();
        ChangePhase(TourneyPhase.beforeRace);
    }
    public void ChangePhase(TourneyPhase nPhase)
    {
        Inspect("Change Phase " + nPhase);
        switch (nPhase)
        {
            case TourneyPhase.beforeRace:
                int season = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;
                if (GetCurrentRaceIndex() % season == season - 1)
                {
                    foreach (var racer in leaderboard.Keys.ToArray())
                        leaderboard[racer] = 0;
                }
                ongoingRace = new Race()
                {
                    raceID = ongoingRace == null ? 0 : ongoingRace.raceID + 1,
                    racers = leaderboard.Keys.Select(k => k).ToList(),
                    lapDistance = DifficultyDefines.lapDistanceBase + DifficultyDefines.lapDistanceAdd * GetCurrentRaceIndex() 
                };
                if (GetCurrentRaceIndex() % RaceDefines.SeasonRaces == 0)
                {
                    PickRandomEnvironment();
                }
                ArenaController.main.parallax.SetWorldDelta(0);
                ArenaController.main.parallax.FromEnvironment(tournamentEnvironment);
                if (ongoingRace.raceID % (RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons) == RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons - 1)
                {
                    ongoingRace.modifier = (RaceDefines.RaceModifiers)Mathf.FloorToInt((int)RaceDefines.RaceModifiers.Elite + Random.value * ((int)RaceDefines.RaceModifiers.Total - (int)RaceDefines.RaceModifiers.Elite));
                }
                else if (ongoingRace.raceID % (RaceDefines.SeasonRaces) == RaceDefines.SeasonRaces - 1)
                {
                    ongoingRace.modifier = (RaceDefines.RaceModifiers)Mathf.FloorToInt(1 + Random.value * ((int)RaceDefines.RaceModifiers.Elite - 1));
                }
                break;
            case TourneyPhase.setup:
                if (currentPhase == TourneyPhase.beforeRace || !ongoingRace.IsRunning())
                {
                    raceCountdown.Set(3);
                    foreach (var racer in ongoingRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceSetup);
                    }
                }
                break;
            case TourneyPhase.racing:
                if (currentPhase == TourneyPhase.setup || !ongoingRace.IsRunning())
                {
                    Inspect($"Start race {ongoingRace.raceID} with {ongoingRace.racers.Count} racers!");
                    foreach (var racer in ongoingRace.racers)
                    {
                        if (currentPhase == TourneyPhase.beforeRace)
                            racer.HandleRacePhase(RaceDefines.RacePhase.RaceSetup);
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceBegin);
                    }
                    StopAllCoroutines();
                    float raceTime = RaceDefines.raceLength;
                    debugSet = 0;

                    var player = GetPlayerRacer();
                    switch (ongoingRace.modifier)
                    {
                        case RaceDefines.RaceModifiers.FasterRival:
                            player.modifiers.Add(new Modifier(player, properties: new Dictionary<ModifierDefines.Property, float>()
                            {
                                {  ModifierDefines.Property.rival_speed, 2 }
                            }));
                            break;
                        case RaceDefines.RaceModifiers.FuelCosnumption:
                            player.modifiers.Add(new Modifier(player, properties: new Dictionary<ModifierDefines.Property, float>()
                            {
                                {  ModifierDefines.Property.fuel_consumption_total,1.5f }
                            }));
                            break;
                        case RaceDefines.RaceModifiers.ActiveCooldown:
                            player.modifiers.Add(new Modifier(player, properties: new Dictionary<ModifierDefines.Property, float>()
                            {
                                {  ModifierDefines.Property.ability_cooldown,2f }
                            }));
                            break;
                        case RaceDefines.RaceModifiers.EngineCooldown:
                            player.modifiers.Add(new Modifier(player, properties: new Dictionary<ModifierDefines.Property, float>()
                            {
                                {  ModifierDefines.Property.engine_cooldown,1.5f },
                                {  ModifierDefines.Property.nitro_cooldown,1.5f }
                            }));
                            break;
                        case RaceDefines.RaceModifiers.LapsLonger:
                            ongoingRace.lapDistance *= 2;
                            break;
                        case RaceDefines.RaceModifiers.LongerRace:
                            raceTime *= RaceDefines.raceLengthLong;
                            break;
                        case RaceDefines.RaceModifiers.RandomEngine:
                        case RaceDefines.RaceModifiers.RandomGadget:
                            var validparts = ResourceCache.main.parts.Where(
                                p => (int)p.boonRarity == Mathf.Min((int)ItemDefines.BoonRarity.legendary, Mathf.FloorToInt(ongoingRace.raceID / RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons))
                                && p.partType == (ongoingRace.modifier == RaceDefines.RaceModifiers.RandomEngine ? ItemDefines.PartType.engine : ItemDefines.PartType.gadget)
                                ).ToArray();

                            if (validparts.Length > 0)
                            {
                                foreach (var racer in ongoingRace.racers)
                                {
                                    if (racer.id != 0)
                                    {
                                        var rSelected = validparts[Mathf.FloorToInt(Random.value * validparts.Length)];
                                        racer.abilities.AddPart(rSelected);
                                    }
                                }
                            }
                            break;
                    }
                    ongoingRace.Set(raceTime);
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
        leaderboard.Clear();
        leaderboard.Add(new PlayerRacer(DataItemPlayer.main.car), 0);
        for (int i = 0; i < opponents; i++)
        {
            leaderboard.Add(new AiRacer(i + 1), 0);
        }
        ArenaController.main?.Clear();
        ArenaController.main?.LoadRacers(leaderboard.Keys.ToArray());
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
        if (currentPhase == TourneyPhase.setup)
        {
            if (!raceCountdown.IsRunning())
                ChangePhase(TourneyPhase.racing);

        }
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
        return !IsLastRaceInSeason() || GetRacerPosition(GetPlayerRacer()) == 0;
    }
    public int GetRacerPosition(Racer racer)
    {
        if (leaderboard == null || leaderboard.Count == 0|| racer==null ||!leaderboard.TryGetValue(racer, out float targetScore))
            return 9;
        return  leaderboard.Count(kvp => kvp.Value > targetScore);
    }
    void HandlePlayerReward()
    {
        int raceTotal = Mathf.Min(EconomyDefines.goldPerRaceLimit, GetCurrentRaceIndex());
        float diffMult = Mathf.Pow(EconomyDefines.goldPerRaceIncrease, raceTotal);

        if (ongoingRace.raceID % DifficultyDefines.eliteRaceInterval == DifficultyDefines.eliteRaceInterval - 1)
            diffMult *= DifficultyDefines.eliteRaceMultiplier;

        DataItemPlayer.main.score.GiveChaos(ItemDefines.chaosPerRace * diffMult);

        int playerPos = ongoingRace.GetPositionForRacer(GetPlayerRacer());
        float interest = Mathf.Clamp(DataItemPlayer.main.econ.gold.GetValue() * (EconomyDefines.constantGoldInterest + DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_interest) - 1), 0, EconomyDefines.interestGoldCap);


        float finishGold = Mathf.Max(0, EconomyDefines.constantGoldForRace * diffMult + DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_bonus));
        float positionGold = Mathf.Floor(EconomyDefines.constantGoldPerPosition * (ongoingRace.racers.Count - playerPos)) * diffMult * DataItemPlayer.main.GetPropertySpeculative(ModifierDefines.Property.gold_income);

        float outputGold = finishGold + positionGold;

        float distanceGold = 0;
        if (playerPos == 0)
        {
            distanceGold = Mathf.Min(Mathf.FloorToInt((ongoingRace.racers[0].position.distanceTraveled - ongoingRace.racers[1].position.distanceTraveled) * EconomyDefines.constantGoldPerDistance),EconomyDefines.performanceGoldCap);
        }

        DataItemPlayer.main.scope.SetVariable("gold_race", finishGold);
        DataItemPlayer.main.scope.SetVariable("gold_position", positionGold);
        DataItemPlayer.main.scope.SetVariable("gold_interest", interest);
        DataItemPlayer.main.scope.SetVariable("gold_performance", distanceGold);

        var GE = DataItemPlayer.main.scope.GetVariable("gold_earned");
        GE.SetFloatValue(GE.GetFloatValue() + interest + outputGold + distanceGold);

        Inspect($"Give player {interest + outputGold + distanceGold} gold; {outputGold} base, {distanceGold} performance and {interest} interest");

        DataItemPlayer.main.econ.GiveGold(interest + outputGold + distanceGold);
    }

    void UpdateVariables()
    {
        var playerRacer = GetPlayerRacer();

        DataItemPlayer.main.scope.SetVariable("race_position_" + ongoingRace.raceID, ongoingRace.GetPositionForRacer(playerRacer));
        DataItemPlayer.main.scope.SetVariable("race_distance_" + ongoingRace.raceID, playerRacer.position.distanceTraveled);
        DataItemPlayer.main.scope.SetVariable("race_topspeed_" + ongoingRace.raceID, playerRacer.stats.realSpeed);

        if (IsLastRaceInSeason())
        {
            var tournamentsCompleted = PlayerConfig.main.globalScope.GetVariable("seasons_completed");
            tournamentsCompleted.SetFloatValue(tournamentsCompleted.GetFloatValue() + 1);
            if (ongoingRace.GetPositionForRacer(playerRacer) == 0)
            {
                var tournamentsWon = PlayerConfig.main.globalScope.GetVariable("seasons_won");
                tournamentsWon.SetFloatValue(tournamentsCompleted.GetFloatValue() + 1);

                var characterWins = PlayerConfig.main.globalScope.GetVariable("seasons_won_with_" + DataItemPlayer.main.car.scriptable.InternalName);
                characterWins.SetFloatValue(tournamentsCompleted.GetFloatValue() + 1);
            }
        }
    }
    void PickRandomEnvironment()
    {
        var environments = ResourceCache.main.environments.Where(e => e != tournamentEnvironment).ToArray();
        if (environments.Length > 0)
            tournamentEnvironment = ResourceCache.main.environments[Mathf.FloorToInt(Random.value * environments.Length)];
    }

    float debugSet = 0;
    void DebugRace()
    {
        int x = 2;
        if (ongoingRace.GetTimeRemaining() < 20 - debugSet * x)
        {
            Inspect($"--- RACE UPDATE ({debugSet * x}) ---");
            int i = 0;
            foreach (var racer in ongoingRace.racers)
            {
                Inspect($"Racer {racer.id} is in position {i + 1} with {racer.position.distanceTraveled} distance going fast at {racer.stats.baseSpeed} km/h! Fuel: {racer.abilities.fuel.GetValue()}/{racer.abilities.fuel.GetLimit()}");
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
    public RaceDefines.RaceModifiers modifier = RaceDefines.RaceModifiers.Nothing;
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
        return GetRivalDistance(raceID , modifier == RaceDefines.RaceModifiers.LongerRace ? RaceDefines.raceLengthLong : RaceDefines.raceLength);
    }
    public static  float GetRivalDistance(int raceID, float raceDuration = RaceDefines.raceLength)
    {
        var baseSpeed = DifficultyDefines.enemyBaseSpeed + DifficultyDefines.enemyWheelSpeed * raceID ;
        var engineSpeed = raceID > 2 ? (DifficultyDefines.enemyEngineSpeed*raceID-2) : 0;
        float engineCooldown = DifficultyDefines.enemyEngineCooldown - DifficultyDefines.enemyEngineDelta;
        return GetRivalDistance(baseSpeed, engineSpeed, engineCooldown, raceDuration );
    }
    public static float GetRivalDistance(float bspd, float espd, float encd, float rt)
    {
        float euse = Mathf.Floor(rt / encd);
        float eint = rt - euse * encd;
        return bspd * rt
                    + espd * encd * (euse * (euse - 1) / 2f)
                    + espd * euse * eint / 2f;
    }
}