
public class PositionLeaderboard : Leaderboard
{
    private void Update()
    {
        UpdateLeaderboard();
    }
    public override void UpdateLeaderboard()
    {
        if (TourneyController.main?.ongoingRace == null) return;
        var racers = TourneyController.main.ongoingRace.racers;

        for (int i = 0; i < entries.Count; i++)
        {
            if (i < racers.Count)
            {
                entries[i].ShowRacerPosition(racers[i], i);
                entries[i].gameObject.SetActive(true);
            }
            else
            {
                entries[i].gameObject.SetActive(false);
            }
        }
    }
}
