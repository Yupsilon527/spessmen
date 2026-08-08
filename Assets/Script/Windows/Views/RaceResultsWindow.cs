using TMPro;
using UnityEngine;

public class RaceResultsWindow : MonoBehaviour
{
    public TextMeshProUGUI  playerPosition;
    public RaceTooltip raceTooltip;

     void OnEnable()
    {
        Refresh();
    }
    public virtual void Refresh()
    {
        UpdatePlayerPosition();
        raceTooltip?.ShowCurrentRace();
    }
    public void UpdatePlayerPosition()
    {
        if (playerPosition != null)
        {
            int position = TourneyController.main?.ongoingRace?.GetPositionForRacer(TourneyController.main.GetPlayerRacer()) ?? 0;

            string posString = LanguageController.main.Translate("Leaderboard", "playerPosition").Replace("%position%", (position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th"));
            playerPosition.text = posString;
        }
    }
}
