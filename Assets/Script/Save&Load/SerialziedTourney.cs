using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SerialziedTourney : SerializableData<TourneyController>
{
    public SerializedPlayer player;
    public List<float> leaderboard;
    public List<string> opponents;
    public int raceId, raceModifier;
    public SerialziedTourney(TourneyController data) : base(data)
    {
        player = new SerializedPlayer(DataItemPlayer.main);
        leaderboard = data.leaderboard.Values.ToList();
        opponents = data.leaderboard.Keys.Select(r=> r.playerShip.InternalName).ToList();

        raceId = data.GetCurrentRaceIndex();
        raceModifier = (int)(data.ongoingRace?.modifier ?? 0);
    }
    public override void Deserialize(TourneyController output)
    {
        base.Deserialize(output);
        output. ChangePhase(TourneyController.TourneyPhase.beforeRace);
    }
}
