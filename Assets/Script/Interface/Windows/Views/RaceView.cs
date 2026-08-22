
using TMPro;
using UnityEngine;

public class RaceView : ViewBase
{
    public TextMeshProUGUI time;
    public PlayerShipGrid playership;
    public PlayerAbilityPreview preview;
    public GameObject interfaceParent;
    public GameObject gameOverParent;
    public GameObject seasonResultsParent;
    public PartTooltip tooltip;
    public PlayerStats playerOverlay;

    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.car);
        preview.LoadPlayerShip(DataItemPlayer.main.car);
        playerOverlay.AssignRacer(TourneyController.main.GetPlayerRacer());
        ToggleGameOver(false);
    }
    void Update()
    {
        UpdateTime();
    }

    void ToggleGameOver(bool value)
    {
        interfaceParent.SetActive(!value);
        gameOverParent.SetActive(value && !TourneyController.main.IsLastRaceInSeason());
        seasonResultsParent.SetActive(value && TourneyController.main.IsLastRaceInSeason());
        if (value)
        {
            preview.Clear();
        }
    }

    void UpdateTime()
    {
        if (time != null)
        {
            if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.setup)
            {
                time.text = TourneyController.main.raceCountdown.GetTimeRemaining().ToString("F1");
            }
            else if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.racing)
            {
                time.text = TourneyController.main.ongoingRace.GetTimeRemaining().ToString("F1");
            }
            else if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.afterRace)
            {
                ToggleGameOver(true);
                time.text = "";
            }
        }
    }
    public void Proceed()
    {
        if (!TourneyController.main.ongoingRace.IsRunning())
        {
            TourneyController.main.ChangePhase(TourneyController.TourneyPhase.beforeRace);
            ViewManager.Instance.ChangeView(ViewManager.Views.shopView);
        }
    }
}
