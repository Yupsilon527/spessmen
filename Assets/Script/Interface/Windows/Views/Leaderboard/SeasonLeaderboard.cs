using UnityEngine;

public class SeasonLeaderboard : Leaderboard
{
    public void OnEnable()
    {
        if (TourneyController.main?.currentPhase == TourneyController.TourneyPhase.afterRace)
        {
            UpdateLeaderboard();
        }
    }
    public override void UpdateLeaderboard()
    {
        int races = RaceDefines.SeasonRaces * RaceDefines.TournamentSeasons;
        int start = Mathf.FloorToInt(TourneyController.main.GetCurrentRaceIndex() / races) * races;

        for (int i = 0; i <  races; i++)
        {
                entries[i].ShowPlayerResults( i+ start);
                entries[i].gameObject.SetActive(true);
        }
    }
}
