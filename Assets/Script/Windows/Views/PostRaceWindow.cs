using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostRaceWindow : RaceResultsWindow
{
    public Button endButton, continueButton;

    public override void Refresh()
    {
        UpdateButtons();
        base.Refresh();
    }
void UpdateButtons()
    {
        if (continueButton!=null)
        {
            continueButton.interactable = TourneyController.main?.CanPlayerProceed() ?? false;
        }
    }
}
