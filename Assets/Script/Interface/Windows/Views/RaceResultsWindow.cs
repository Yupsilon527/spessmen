using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceResultsWindow : MonoBehaviour
{
    public TextMeshProUGUI  playerPosition, cupTitle;
    public RaceTooltip raceTooltip;
    public Button proceedButton;

     void OnEnable()
    {
        Refresh();
    }
    public virtual void Refresh()
    {
        UpdatePlayerPosition();
        raceTooltip?.ShowCurrentRace();
    }
    public virtual  void UpdatePlayerPosition()
    {
        int position = TourneyController.main?.ongoingRace?.GetPositionForRacer(TourneyController.main.GetPlayerRacer()) ?? 0;
        if (playerPosition != null)
        {
            string posString = LanguageController.main?.Translate("Leaderboard", "playerPosition").Replace("%position%", (position == 0 ? "1<sup>st" : position == 1 ? "2<sup>nd" : position == 2 ? "3<sup>rd" : $"{position + 1}<sup>th"))??"";
            playerPosition.text = posString;
        }
        if (cupTitle!= null)
        {
            int cup = TourneyController.main?.GetCurrentSeason() ?? 0;
            string cupLabel = cup == 0 ? LanguageController.main?.Translate("Leaderboard", "firstcup") :
                cup == 1 ? LanguageController.main?.Translate("Leaderboard", "secondcup") :
                cup == 2 ? LanguageController.main?.Translate("Leaderboard", "thirdcup") : LanguageController.main?.Translate("Leaderboard", "endless");
            cupTitle.text = cupLabel;
        }
        if (proceedButton != null)
            proceedButton.interactable = position <= 2;
    }
}
