using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceView : ViewBase
{
    public PlayerShipGrid playership;
    public PlayerAbilityPreview preview;

    public override void OnOpened()
    {
        base.OnOpened();
        if (DataItemPlayer.main == null) return;
        playership.AssignShip(DataItemPlayer.main.ship);
        preview.LoadPlayerShip(DataItemPlayer.main.ship);
    }
}
