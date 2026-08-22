using UnityEngine;

public class ResetRunButton : ButtonBase
{
    public override void OnPressed()
    {
        PlayerConfig.main.StartNewGame();
    }
}
