using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceResultsWindow : MonoBehaviour
{
    public TextMeshProUGUI  playerPosition, cupTitle, qualifiedText;
    public RaceTooltip raceTooltip;
    public Button proceedButton;
    public Image cupIcon;
    public Sprite cupFirstSprite, cupSecondSprite, cupThirdSprite, cupEndlessSprite;

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
            if (cupIcon != null)
            {
                cupIcon.sprite = cup == 0 ? cupFirstSprite :
                cup == 1 ? cupSecondSprite :
                cup == 2 ? cupThirdSprite : cupEndlessSprite;
            }
        }
        bool qualified = position <= 2;
        if (qualifiedText != null)
            qualifiedText.text = qualified ? LanguageController.main?.Translate("Leaderboard", "qualified") : LanguageController.main?.Translate("Leaderboard", "disqualified");
        if (proceedButton != null)
            proceedButton.interactable = qualified;
    }
}
