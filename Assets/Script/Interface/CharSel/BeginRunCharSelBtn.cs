using UnityEngine;

public class BeginRunCharSelBtn : ButtonBase 
{
    public override void OnPressed()
    {
        RacerSelView.main?.StartTheGame();
    }
}
