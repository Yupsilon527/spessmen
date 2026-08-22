using UnityEngine.UI;

public class PostRaceWindow : RaceResultsWindow
{
    public Button endButton, continueButton;

    public override void Refresh()
    {
        UpdateButtons();
        base.Refresh();
    }
    public override void UpdatePlayerPosition()
    {
        if (playerPosition != null)
        {
            int position = TourneyController.main?.GetRacerPosition(TourneyController.main ?.GetPlayerRacer()) ?? 0;

            string posString = LanguageController.main?.Translate("Leaderboard", "playerPosition").Replace("%position%", (position == 0 ? "1<sup>st" : position == 1 ? "2<sup>nd" : position == 2 ? "3<sup>rd" : $"{position + 1}<sup>th")) ?? "";
            playerPosition.text = posString;
        }
    }
    void UpdateButtons()
    {
        if (continueButton!=null)
        {
            continueButton.interactable = TourneyController.main?.CanPlayerProceed() ?? false;
        }
    }
}
