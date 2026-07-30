
    public class ResultsLeaderboard : Leaderboard
    {
        public void OnEnable()
        {
            if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.afterRace)
            {
                UpdateLeaderboard();
            }
        }
        public override void UpdateLeaderboard()
        {
            var racers = TourneyController.main.GetLeaderboardSorted();


            for (int i = 0; i < entries.Count; i++)
            {
                if (i < racers.Length)
                {
                    entries[i].ShowRacerTournamentStanding(racers[i], i);
                    entries[i].gameObject.SetActive(true);
                }
                else
                {
                    entries[i].gameObject.SetActive(false);
                }
            }
        }
    }
