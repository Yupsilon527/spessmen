using System.Collections.Generic;
using System.Data;
using System.Linq;

public class TourneyController : Initializable
{
    public static TourneyController main;
    Dictionary<Racer, float> leaderboard = new();


    public Race currentRace;
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
                break;
            case TourneyPhase.racing:
                if (currentPhase == TourneyPhase.beforeRace || currentRace == null || !currentRace.IsRunning())
                {
                    currentRace = new Race()
                    {
                        raceID = currentRace == null ? 0 : currentRace.raceID + 1,
                        racers = leaderboard.Keys.Select(k => k).ToList(),
                    };
                    Inspect($"Start race {currentRace.raceID} with {currentRace.racers.Count} racers!");
                    foreach (var racer in currentRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceBegin);
                    }
                    debugSet = 0;
                    currentRace.Set(60);
                }
                break;
            case TourneyPhase.afterRace:
                if (currentPhase == TourneyPhase.racing)
                {
                    foreach (var racer in currentRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceEnd);
                    }
                }
                break;
        }
        currentPhase = nPhase;
    }
    protected override void Initialize()
    {
        main = this;


        base.Initialize();
    }
    public void InitRacers(int opponents = 5)
    {
        leaderboard.Add(new PlayerRacer(DataItemPlayer.main.ship), 0);
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
        if (currentRace == null || currentRace.raceID == 0) return null;
        return leaderboard
            .Where(kvp => kvp.Key.id != 0)
            .OrderBy(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .FirstOrDefault();
    }
    private void FixedUpdate()
    {
        if (currentPhase == TourneyPhase.racing)
        {
            if (currentRace.IsRunning())
            {
                foreach (var racer in currentRace.racers)
                    racer.HandleRacePhase(RaceDefines.RacePhase.RaceTick);
                currentRace.UpdateLeaderboard();
                DebugRace();
            }
            else
                ChangePhase(TourneyPhase.afterRace);
        }
    }
    float debugSet = 0;
    void DebugRace()
    {
        if (currentRace.GetTimeRemaining()<60- debugSet*5)
        {
            Inspect($"--- RACE UPDATE ({debugSet * 5}) ---");
            int i = 0;
            foreach (var racer in currentRace.racers)
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
    public float lapDistance = 300;
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