
using TMPro;
using UnityEngine;

public class RaceView : ViewBase
{
    public TextMeshProUGUI time,playerPosition;
    public PlayerShipGrid playership;
    public PlayerAbilityPreview preview;
    public GameObject interfaceParent;
    public GameObject gameOverParent;
    public PartTooltip tooltip;
    public RaceTooltip raceTooltip;

    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.ship);
        preview.LoadPlayerShip(DataItemPlayer.main.ship);
        ToggleGameOver(false);
    }
    void Update()
    {
        UpdateTime();
    }

    void ToggleGameOver(bool value)
    {
        interfaceParent.SetActive(!value);
        gameOverParent.SetActive(value);
        if (value)
        {
            preview.Clear();
            if (playerPosition != null) {
                int position =  TourneyController.main.ongoingRace.GetPositionForRacer( TourneyController.main.GetPlayerRacer());
                playerPosition.text = "You placed "+ (position == 0 ? "1st" : position == 1 ? "2nd" : position == 2 ? "3rd" : $"{position + 1}th") ;
            }
            raceTooltip?.ShowCurrentRace();
        }
    }

    void UpdateTime()
    {
        if (time != null)
        {
            if (TourneyController.main.currentPhase == TourneyController.TourneyPhase.racing)
            {
                time.text = (Mathf.Ceil(TourneyController.main.ongoingRace.GetTimeRemaining() * 10) / 10).ToString("F1");
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
