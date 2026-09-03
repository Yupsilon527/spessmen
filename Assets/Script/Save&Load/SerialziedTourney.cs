using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SerialziedTourney : SerializableData<TourneyController>
{
    public string environmentName;
    public string versionLabel;
    public SerializedPlayer player;
    public List<float> leaderboard;
    public List<string> opponents;
    public int raceId, raceModifier;
    public SerialziedTourney(TourneyController data) : base(data)
    {
        environmentName = data.tournamentEnvironment.InternalName;
        versionLabel = Application.version;
        player = new SerializedPlayer(DataItemPlayer.main);
        leaderboard = data.leaderboard.Values.ToList();
        opponents = data.leaderboard.Keys.Select(r => r.playerShip.InternalName).ToList();

        raceId = data.GetCurrentRaceIndex();
        raceModifier = (int)(data.ongoingRace?.modifier ?? 0);
    }
    public override void Deserialize(TourneyController output)
    {
        player.Deserialize(DataItemPlayer.main);
        output.tournamentEnvironment = ResourceCache.main.LoadEnvironment(environmentName);//TODO fallback
        output.LoadRacers(opponents, leaderboard);

        output.ongoingRace = new Race(raceId)
        {
            racers = output.leaderboard.Keys.Select(k => k).ToList(),
            modifier = (RaceDefines.RaceModifiers)raceModifier
        };

        output.ChangePhase(TourneyController.TourneyPhase.beforeRace);
    }
}
