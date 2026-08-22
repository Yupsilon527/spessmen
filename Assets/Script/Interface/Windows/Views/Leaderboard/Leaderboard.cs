using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public List<LeaderboardEntry> entries = new();
    public virtual void UpdateLeaderboard()
    {

    }
    private void Reset()
    {
        entries = gameObject.GetComponentsInChildren<LeaderboardEntry>().ToList();
    }
}
