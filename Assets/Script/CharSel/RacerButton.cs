using UnityEngine;
using UnityEngine.UI;

public class RacerButton : RacerSelComponent
{
    public Image portrait;

    public override void AssignScriptable(ShipScriptable ship)
    {
        base.AssignScriptable(ship);
        portrait.sprite = ship.portrait;
    }
    public virtual void OnPress()
    {
        PlayerConfig.main.playerCharacter = assigned;
        RacerSelView.main.AssignScriptable(assigned);
    }
}
