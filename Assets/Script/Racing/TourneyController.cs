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
        switch (nPhase) {
            case TourneyPhase.racing:
                if (currentPhase == TourneyPhase.beforeRace || currentRace == null  || !currentRace.IsRunning())
                {
                    currentRace = new Race()
                    {
                        raceID = currentRace == null ? 0 : currentRace.raceID + 1,
                        racers = leaderboard.Keys.Select(k => k).ToList(),
                    };
                    foreach (var racer in currentRace.racers)
                    {
                        racer.HandleRacePhase(RaceDefines.RacePhase.RaceBegin);
                    }
                    currentRace.Set(60 * 3);
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
        
        InitRacers();
        ChangePhase(TourneyPhase.beforeRace);

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
        if (currentRace.IsRunning())
        {
            foreach (var racer in currentRace.racers)
                racer.OnRaceProgress();
            currentRace.UpdateLeaderboard();
        }
        else if (currentPhase == TourneyPhase.racing)
            ChangePhase(TourneyPhase.afterRace);
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
        racers.Sort((a, b) => a.position.distanceTraveled.CompareTo(b.position.distanceTraveled));
    }
}