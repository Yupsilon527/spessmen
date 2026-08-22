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

        for (int i = start; i < start+ races; i++)
        {
                entries[i].ShowPlayerResults( i);
                entries[i].gameObject.SetActive(true);
        }
    }
}
