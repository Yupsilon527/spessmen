using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public List<LeaderboardEntry> entries = new();

    private void Update()
    {
        UpdatePositionLeaderboard();
    }
    public void UpdatePositionLeaderboard()
    {
        var racers = TourneyController.main.ongoingRace.racers;

        for (int i = 0; i<entries.Count; i++)
        {
            if (i<racers.Count) {
                entries[i].RacerPosition(racers[i], i);
                entries[i].gameObject.SetActive(true);
            }
            else
            {
                entries[i].gameObject.SetActive(false);
            }
        }
    }
}
